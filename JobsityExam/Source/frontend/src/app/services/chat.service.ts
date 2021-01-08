import { Injectable, OnInit } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { HttpClient } from '@angular/common/http';
import { MessageRequest } from '../dto/MessageRequest';
import { Observable, Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ChatService {

  private connection: any = new signalR.HubConnectionBuilder()
                                  .configureLogging(signalR.LogLevel.Debug)
                                  .withUrl("http://localhost:8080/chatsocket", {
                                    transport: signalR.HttpTransportType.WebSockets,
                                    skipNegotiation: true
                                  }) 
                                  .build();

  readonly POST_URL = "http://localhost:8080/api/chat/send"

  private receivedMessageObject: MessageRequest = new MessageRequest();
  private sharedObj = new Subject<MessageRequest>();
  
  constructor(private http: HttpClient) { 
    this.connection.onclose(async () => {
      await this.start();
    });
   this.connection.on("ReceiveOne", (User: string, Message: string) => { this.mapReceivedMessage(User, Message); });
   this.start();                 
  }


  // Strart the connection
  public async start() {
    try {
      await this.connection.start();
      console.log("connected");
    } catch (err) {
      console.log(err);
      setTimeout(() => this.start(), 5000);
    } 
  }

  private mapReceivedMessage(User: string, Message: string): void {
    this.receivedMessageObject.User = User;
    this.receivedMessageObject.Message = Message;
    this.sharedObj.next(this.receivedMessageObject);
 }

  /* ****************************** Public Mehods **************************************** */

  // Calls the controller method
  public broadcastMessage(msgDto: any) {
    this.http.post(this.POST_URL, msgDto).subscribe(data => console.log(data));
  }

  public retrieveMappedObject(): Observable<MessageRequest> {
    return this.sharedObj.asObservable();
  }
}
