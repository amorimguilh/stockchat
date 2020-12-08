import { Component, OnInit } from '@angular/core';
import { MessageRequest } from './dto/MessageRequest';
import { ChatService } from './services/chat.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {

  constructor(private chatService: ChatService) {}

  ngOnInit(): void {
    this.chatService.retrieveMappedObject().subscribe( (receivedObj: MessageRequest) => { this.addToInbox(receivedObj);});
                                                     
  }

  msgDto: MessageRequest = new MessageRequest();
  msgInboxArray: MessageRequest[] = [];

  send(): void {
    if(this.msgDto) {
      if(this.msgDto.Message.length == 0 || this.msgDto.User.length == 0){
        window.alert("Both fields are required.");
        return;
      } else {
        this.chatService.broadcastMessage(this.msgDto);
        this.msgDto.Message = '';
      }
    }
  }

  addToInbox(obj: MessageRequest) {
    let newObj = new MessageRequest();
    newObj.User = obj.User;
    newObj.Message = obj.Message;
    if(this.msgInboxArray.length == 50)
    {
      this.msgInboxArray.shift();
    }
    this.msgInboxArray.push(newObj);
  }
}
