  use master
  go
  Create database TarineeDB
  go
  use  TarineeDB
 go

CREATE TABLE Trainee (
    TraineeID      INT           IDENTITY (1, 1) NOT NULL,
    TraineeName    VARCHAR (100) NULL,
    CourseID INT           NULL,
    DOB        DATE          NULL,
    Gender     VARCHAR (20)  NULL, 
    ImagePath  VARCHAR (250) NULL,
    CONSTRAINT [PK_Trainee] PRIMARY KEY CLUSTERED (TraineeID ASC)
);
go


 use  TarineeDB
 go

create table Payment(
PaymentID  INT IDENTITY (1, 1) NOT NULL,
PaymentDate Date Not null,
PaymentPurpose VARCHAR (150)  NULL, 
PaymentMethod VARCHAR (150)  NULL, 
Amount     INT          NOT NULL,
TraineeID       INT           NULL,
CourseID  INT           NULL,
Note VARCHAR (50)  NULL
);
go

 use  TarineeDB
 go
CREATE TABLE Course (
    CourseID INT  Identity         NOT NULL,
    Course   NVARCHAR (50) NULL,
	CourseFee int NOT NULL,
    PRIMARY KEY CLUSTERED (CourseID ASC)
);
go

 use  TarineeDB
 go

CREATE PROC PaymentAddOrEdit
@PaymentID int,
@TraineeID int,
@CourseID int,
@PaymentDate date,
@PaymentPurpose Varchar(150),
@PaymentMethod Varchar(150),
@Amount int,
@Note Varchar(50)
AS
	--Insert
	IF @PaymentID = 0
		INSERT INTO Payment(TraineeID,CourseID,PaymentDate,PaymentPurpose,PaymentMethod,Amount, Note)
		VALUES (@TraineeID,@CourseID,@PaymentDate,@PaymentPurpose,@PaymentMethod,@Amount, @Note)
	--Update
	ELSE
		UPDATE Payment
		SET
			TraineeID=@TraineeID,
			CourseID=@CourseID,
			PaymentDate=@PaymentDate,
			PaymentPurpose=@PaymentPurpose,
			PaymentMethod=@PaymentMethod,
			Amount=@Amount,
			Note=@Note
		WHERE PaymentID = @PaymentID
go

 use  TarineeDB
 go
CREATE PROC PaymentDelete
@PaymentID int
AS
	DELETE FROM Payment
	WHERE PaymentID = @PaymentID
go


 use  TarineeDB
 go
CREATE PROC TraineeAddOrEdit
@TraineeID int,
@TraineeName varchar(100),
@CourseID int,
@DOB date,
@Gender varchar(20),
@ImagePath varchar(250)
AS

	--Insert
	IF @TraineeID = 0 BEGIN
		INSERT INTO Trainee(TraineeName,CourseID,DOB,Gender,ImagePath)
		VALUES (@TraineeName,@CourseID,@DOB,@Gender,@ImagePath)

		SELECT SCOPE_IDENTITY();

		END
	--Update
	ELSE BEGIN
		UPDATE Trainee
		SET
			
			TraineeName=@TraineeName,
			CourseID=@CourseID,
			DOB=@DOB,
			Gender=@Gender,
			
			ImagePath=@ImagePath
		WHERE TraineeID=@TraineeID

		SELECT @TraineeID;

		END
		go

 use  TarineeDB
 go
CREATE PROC TraineeDelete
@TraineeID int
AS
	--Master
	DELETE FROM Trainee
	WHERE TraineeID = @TraineeID
	--Details
	DELETE FROM Payment
	WHERE TraineeID = @TraineeID
go

 use  TarineeDB
 go
CREATE PROC TraineeViewAll
AS
SELECT T.TraineeID,T.TraineeName,T.Gender,T.DOB,T.ImagePath
FROM Trainee T 
go

 use  TarineeDB
 go
create Proc PaymentViewAll
as
select *
from Payment
go

 use  TarineeDB
 go
CREATE PROC TraineeViewByID
@TraineeID int
AS
	--Master
	SELECT *
	FROM Trainee
	WHERE TraineeID = @TraineeID
	--Details
	SELECT *
	FROM Payment
	WHERE TraineeID = @TraineeID
	go



