﻿-- CREATE TABLE
CREATE TABLE CAMERA_STATUS_HISTORY
(
	ID             				NUMBER(18) NOT NULL,
	CAMERA_ID					NUMBER(18) NOT NULL,	
	CAMERA_STATUS_TYPE_ID		NUMBER NOT NULL,
	CREATED_TIMESTAMP			DATE NOT NULL
);
-- CREATE/RECREATE PRIMARY, UNIQUE AND FOREIGN KEY CONSTRAINTS 
ALTER TABLE CAMERA_STATUS_HISTORY ADD CONSTRAINT CAMERA_STATUS_HISTORY_PK PRIMARY KEY (ID) USING INDEX;

-- SEQUENCE
CREATE SEQUENCE  CAMERA_STATUS_HISTORY_SEQ;

-- TRIGGER
CREATE OR REPLACE TRIGGER CAMERA_STATUS_HISTORY_BI BEFORE INSERT ON CAMERA_STATUS_HISTORY FOR EACH ROW
BEGIN
	:NEW.ID := CAMERA_STATUS_HISTORY_SEQ.NEXTVAL;
END CAMERA_STATUS_HISTORY_BI;
/