--SELECT * FROM Tickets;

--SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE';

--SELECT * FROM Projects;

--SELECT
--	Tickets.title,
--	Projects.title,
--	AspNetUsers.FirstName
--FROM
--	Tickets
--INNER JOIN
--	Projects ON Projects.Id = Tickets.ProjectId
--LEFT JOIN
--	AspNetUsers ON AspNetUsers.Id = Tickets.AssignToUserId;

--INSERT INTO ApplicationUserProjects (Project_Id, ApplicationUser_Id) VALUES ('4', '7');