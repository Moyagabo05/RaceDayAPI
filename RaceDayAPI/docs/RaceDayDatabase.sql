--Creating The Database
 CREATE DATABASE RaceDay;

 --Creating Users Table

 CREATE TABLE Users (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    Email VARCHAR(255) NOT NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL,
    Role VARCHAR(50) NOT NULL CHECK (Role IN ('Organiser', 'Participant')),
    CreatedAt DATETIME DEFAULT GETDATE()
);
Select * FROM Users;

--Creating Organisers Table
CREATE TABLE Organisers (
    OrganiserID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NOT NULL UNIQUE, -- UNIQUE enforces 1-to-1 relationship with Users
    FirstName VARCHAR(150) NOT NULL,
    LastName VARCHAR(150) NOT NULL,
    OrganisationName VARCHAR(150) NULL,
    PhoneNumber VARCHAR(11) NOT NULL,
    CONSTRAINT FK_Organisers_Users FOREIGN KEY (UserID) REFERENCES Users(UserID)
);
SELECT * FROM Organisers

--Creating Participants Table
CREATE TABLE Participants (
    ParticipantID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NOT NULL UNIQUE, -- UNIQUE enforces 1-to-1 relationship with Users
    FirstName VARCHAR(150) NOT NULL,
    LastName VARCHAR(150) NOT NULL,
    DateOfBirth DATE NOT NULL,
    Gender VARCHAR(10) NOT NULL,
    EmergencyContact VARCHAR(200) NOT NULL,
    ClubName VARCHAR(200) NULL,
    CONSTRAINT FK_Participants_Users FOREIGN KEY (UserID) REFERENCES Users(UserID)
);
--SELECT * FROM Participants;

--Creating Events Table
CREATE TABLE Events (
    EventID INT IDENTITY(1,1) PRIMARY KEY,
    OrganiserID INT NOT NULL,
    Name VARCHAR(200) NOT NULL,
    Description TEXT NOT NULL,
    EventDate DATETIME NOT NULL,
    Location VARCHAR(200) NOT NULL,
    Distance DECIMAL(5,2) NOT NULL, -- For example 10.50 for 10.5km
    EventType VARCHAR(150) NOT NULL CHECK (EventType IN ('Run', 'Walk', 'Cycle')),
    BannerImageUrl VARCHAR(500) NULL,
    CONSTRAINT FK_Events_Organisers FOREIGN KEY (OrganiserID) REFERENCES Organisers(OrganiserID)
);
SELECT * FROM Events;

--Creating Categories Table
CREATE TABLE Categories (
    CategoryID INT IDENTITY(1,1) PRIMARY KEY,
    EventID INT NOT NULL,
    CategoryName VARCHAR(150) NOT NULL,
    MinAge INT NULL,
    MaxAge INT NULL,
    CONSTRAINT FK_Categories_Events FOREIGN KEY (EventID) REFERENCES Events(EventID)
);
SELECT * FROM Categories;

--Creating Enrolments Table
CREATE TABLE Enrolments (
    EnrolmentID INT IDENTITY(1,1) PRIMARY KEY,
    ParticipantID INT NOT NULL,
    EventID INT NOT NULL,
    CategoryID INT NOT NULL,
    Status VARCHAR(50) NOT NULL DEFAULT 'Pending' CHECK (Status IN ('Pending', 'Confirmed', 'Cancelled')),
    EnrolmentDate DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Enrolments_Participants FOREIGN KEY (ParticipantID) REFERENCES Participants(ParticipantID),
    CONSTRAINT FK_Enrolments_Events FOREIGN KEY (EventID) REFERENCES Events(EventID),
    CONSTRAINT FK_Enrolments_Categories FOREIGN KEY (CategoryID) REFERENCES Categories(CategoryID)
);
Select * FROM Enrolments;

--Creating Results Table
CREATE TABLE Results (
    ResultID INT IDENTITY(1,1) PRIMARY KEY,
    EnrolmentID INT NOT NULL UNIQUE, -- UNIQUE enforces 1-to-1 relationship with Enrolments
    FinishTime TIME NULL,
    Position INT NULL,
    CONSTRAINT FK_Results_Enrolments FOREIGN KEY (EnrolmentID) REFERENCES Enrolments(EnrolmentID)
);
SELECT * FROM Results;

--Inserting into Users Table
INSERT INTO Users (Email, PasswordHash, Role) VALUES
('org.maya.raphasha@raceday.co.za', '$2a$11$dummyhashformaya123456789012345678901234567890', 'Organiser'),
('org.jeff.sehlwane@raceday.co.za', '$2a$11$dummyhashforjeff12345678901234567890123456789', 'Organiser'),
('part.kea.runner@raceday.co.za', '$2a$11$dummyhashforkea123456789012345678901234567890', 'Participant'),
('part.nomsa.walker@raceday.co.za', '$2a$11$dummyhashfornomsa12345678901234567890123456789', 'Participant');

SELECT * FROM Users;

-- Inserting into Organisers Table 
INSERT INTO Organisers (UserID, FirstName, LastName, OrganisationName, PhoneNumber) VALUES
(1, 'Maya', 'Raphasha', 'Joburg Road Runners', '0612127353'),
(2, 'Jeff', 'Sehlwane', 'Cape Town Cycle Club', '0720168646');

SELECT * FROM Organisers;

-- Inserting into  Participants Table
INSERT INTO Participants (UserID, FirstName, LastName, DateOfBirth, Gender, EmergencyContact, ClubName) VALUES
(3, 'Kea', 'Runner', '2005-03-15', 'Female', '0845647543', 'Nedbank Running Club'),
(4, 'Nomsa', 'Walker', '1980-10-21', 'Female', '0873452332', NULL);

SELECT * FROM Participants;


-- Inserting into  Events Table 
INSERT INTO Events (OrganiserID, Name, Description, EventDate, Location, Distance, EventType) VALUES
(1, 'Joburg Spring 10km', 'A scenic 10km run through the Johannesburg Botanical Gardens.', '2026-11-11 09:30:00', 'Johannesburg Botanical Gardens', 20.00, 'Run'),
(1, 'Montana Charity Walk', 'A 5km community walk to raise funds for local schools.', '2026-11-02 07:45:00', 'Doornpoort, Montana', 15.00, 'Walk'),
(2, 'Cape Point Cycle Tour', 'A challenging 50km cycle route along the beautiful coastline.', '2026-12-01 06:15:00', 'Cape Point, Cape Town', 30.00, 'Cycle');

SELECT * FROM Events;

-- Inserting into Categories for the Events
-- Event 1 (Joburg 10km) Categories
INSERT INTO Categories (EventID, CategoryName, MinAge, MaxAge) VALUES
(1, 'Under 21', 10, 21),
(1, 'Senior (22-40)', 22, 40),
(1, 'Veteran (41+)', 41, 100);
SELECT * FROM Categories;

-- Event 2 (Montana Walk) Categories
INSERT INTO Categories (EventID, CategoryName, MinAge, MaxAge) VALUES
(2, 'Open', 18, 100),
(2, 'Youth', 12, 25);

SELECT * FROM Categories;

-- Event 3 (Cape Cycle Tour) Categories
INSERT INTO Categories (EventID, CategoryName, MinAge, MaxAge) VALUES
(3, '30km Elite', 18, 50),
(3, '30km Amateur', 18, 100);

SELECT * FROM Categories;


-- Insert Enrolments
-- Kea (Participant 1) enrolls in Joburg 20km (Event 1, Category 2: Senior)
INSERT INTO Enrolments (ParticipantID, EventID, CategoryID, Status) VALUES
(1, 1, 2, 'Confirmed');


-- Nomsa (Participant 2) enrolls in Montana Walk (Event 2, Category 1: Open)
INSERT INTO Enrolments (ParticipantID, EventID, CategoryID, Status) VALUES
(2, 2, 1, 'Pending');



-- Kea (Participant 1) enrolls in Cape Cycle Tour (Event 3, Category 2: Amateur)
INSERT INTO Enrolments (ParticipantID, EventID, CategoryID, Status) VALUES
(1, 3, 2, 'Pending');

SELECT * FROM Enrolments;

-- Insert Results (For completed events)
-- Kea finished the Joburg 20km in 30 mins, 30 secs (5th position)
INSERT INTO Results (EnrolmentID, FinishTime, Position) VALUES
(1, '00:30:30', 12);


-- Nomsa finished the Monata Walk in 50 mins, 15 secs (100th position)
INSERT INTO Results (EnrolmentID, FinishTime, Position) VALUES
(2, '00:50:15', 100);
SELECT * FROM Results;




