# stockchat
Realtime chat with options to get the price of a given stock

!!! Right now the solution is only working a local machine due to some cors problems between the frontend and the chat application.

Dependencies:
 - Docker
 - .Net Core development environment

HOW TO RUN THIS SOLUTION
  1) Having docker running, on a command line tool, create a container to host RabbitMQ with the command: 
     docker run -d --hostname rabbit --name rabbit --domainname rabbit -p 5672:5672 -p 15672:15672 --env RABBITMQ_DEFAULT_USER=user --env RABBITMQ_DEFAULT_PASS=mysecretpassword  rabbitmq:3-management
  2) Open 4 poweshell instances (it can be any command line tool)
  3) On the first instance change the directory to: <clonned solution path>\JobsityExam\Source\StockConsumer
  4) Run the command 'dotnet run'.
  5) On the second powershell instance change the directory to: <clonned solution path>\JobsityExam\Source\ChatApplication
  6) Run the command 'dotnet run'.
  7) On the third powershell instance change the directory to: <clonned solution path>\JobsityExam\Source\RabbitMqWorker
  8) Run the command 'dotnet run'.
  9) On the fourth powershell instance change the directory to: <clonned solution path>\JobsityExam\Source\frontend
 10) Run the command 'npm start'.
 
The Application should be available at localhost:4200  
