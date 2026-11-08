-- 1. CLEAR ALL EXISTING DATA (In order of dependencies)
DELETE FROM Results;
DELETE FROM Enrolments;
DELETE FROM Categories;
DELETE FROM Events;
DELETE FROM Participants;
DELETE FROM Organisers;
DELETE FROM Users;

-- 2. RESET ID COUNTERS BACK TO 1
DBCC CHECKIDENT ('Results', RESEED, 0);
DBCC CHECKIDENT ('Enrolments', RESEED, 0);
DBCC CHECKIDENT ('Categories', RESEED, 0);
DBCC CHECKIDENT ('Events', RESEED, 0);
DBCC CHECKIDENT ('Participants', RESEED, 0);
DBCC CHECKIDENT ('Organisers', RESEED, 0);
DBCC CHECKIDENT ('Users', RESEED, 0);

-- 3. INSERT FRESH SEED DATA
INSERT INTO Users (Email, PasswordHash, Role, CreatedAt) VALUES
('org.maya.raphasha@raceday.co.za', '$2a$11$dummyhashformaya123456789012345678901234567890', 'Organiser', GETDATE()),
('org.jeff.sehlwane@raceday.co.za', '$2a$11$dummyhashforjeff12345678901234567890123456789', 'Organiser', GETDATE()),
('part.kea.runner@raceday.co.za', '$2a$11$dummyhashforkea123456789012345678901234567890', 'Participant', GETDATE()),
('part.nomsa.walker@raceday.co.za', '$2a$11$dummyhashfornomsa12345678901234567890123456789', 'Participant', GETDATE());

INSERT INTO Organisers (UserID, FirstName, LastName, OrganisationName, PhoneNumber) VALUES
(1, 'Maya', 'Raphasha', 'Joburg Road Runners', '0612127353'),
(2, 'Jeff', 'Sehlwane', 'Cape Town Cycle Club', '0720168646');

INSERT INTO Participants (UserID, FirstName, LastName, DateOfBirth, Gender, EmergencyContact, ClubName) VALUES
(3, 'Kea', 'Runner', '2005-03-15', 'Female', '0845647543', 'Nedbank Running Club'),
(4, 'Nomsa', 'Walker', '1980-10-21', 'Female', '0873452332', NULL);

INSERT INTO Events (OrganiserID, Name, Description, EventDate, Location, Distance, EventType) VALUES
(1, 'Joburg Spring 10km', 'A scenic 10km run through the Johannesburg Botanical Gardens.', '2026-11-11 09:30:00', 'Johannesburg Botanical Gardens', 20.00, 'Run'),
(1, 'Montana Charity Walk', 'A 5km community walk to raise funds for local schools.', '2026-11-02 07:45:00', 'Doornpoort, Montana', 15.00, 'Walk'),
(2, 'Cape Point Cycle Tour', 'A challenging 50km cycle route along the beautiful coastline.', '2026-12-01 06:15:00', 'Cape Point, Cape Town', 30.00, 'Cycle');

INSERT INTO Categories (EventID, CategoryName, MinAge, MaxAge) VALUES
(1, 'Under 21', 10, 21), (1, 'Senior', 22, 40), (1, 'Veteran', 41, 100),
(2, 'Open', 18, 100), (2, 'Youth', 12, 25),
(3, 'Elite', 18, 50), (3, 'Amateur', 18, 100);

INSERT INTO Enrolments (ParticipantID, EventID, CategoryID, Status, EnrolmentDate) VALUES
(1, 1, 2, 'Confirmed', GETDATE()), 
(2, 2, 1, 'Pending', GETDATE()), 
(1, 3, 2, 'Pending', GETDATE());

INSERT INTO Results (EnrolmentID, FinishTime, Position) VALUES
(1, '00:30:30', 12), 
(2, '00:50:15', 100);
