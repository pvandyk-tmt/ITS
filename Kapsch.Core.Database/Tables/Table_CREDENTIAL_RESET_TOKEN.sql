﻿-- CREATE TABLE
CREATE TABLE CREDENTIAL_RESET_TOKEN
(
	ID					NUMBER(18) NOT NULL,
	CREDENTIAL_ID       NUMBER(18) NOT NULL,
	EXPIRY_TIMESTAMP    DATE NOT NULL,
	TOKEN         		VARCHAR2(512) NOT NULL	
);
-- CREATE/RECREATE PRIMARY, UNIQUE AND FOREIGN KEY CONSTRAINTS 
ALTER TABLE CREDENTIAL_RESET_TOKEN ADD CONSTRAINT CREDENTIAL_RESET_TOKEN_PK PRIMARY KEY (ID)  USING INDEX;

-- SEQUENCE
CREATE SEQUENCE  CREDENTIAL_RESET_TOKEN_SEQ;

-- TRIGGER
CREATE OR REPLACE TRIGGER CREDENTIAL_RESET_TOKEN_BI BEFORE INSERT ON CREDENTIAL_RESET_TOKEN FOR EACH ROW
BEGIN
    :NEW.ID := CREDENTIAL_RESET_TOKEN_SEQ.NEXTVAL;
END CREDENTIAL_RESET_TOKEN_BI;
/