For this task we need to develop a backend application that will provide the necessary API for a project inspired by the management board applications. 
Develop this application in C# .NET core 8.0 using the Entity Framework, MSSql and RabbitMq (event driven communication).

This application will have the following functionality: 

• Provide a CRUD Project API: a project will contain a title, description, name, progress (0-100), start date and finish date.  
• Provide a CRUD Job API: a job will contain a title, description, project id, progress (0-100), status (TODO, IN-PROGRESS, IN-REVIEW, DONE), start date and finish date.  
• When a Project is retrieved, the jobs with the same project id should be retrieved as well.  
• Provide a Swagger API documentation for the application.  
• The application and database must run in docker containers and use docker-compose. 
  
Advanced Considerations: 

• Code quality: Incorporate best practices, design patterns. 
• Innovation and Extension: encouraged to introduce additional features or technologies that enhance the application's functionality, usability, or scalability.  
• Security and Performance: Demonstrate the understanding of security best practices and performance optimization in API development.
