﻿-- CREATE TABLE
CREATE TABLE CAMERA_LAST_STATISTICS
(
	ID             										NUMBER(18) NOT NULL,
    CAMERA_ID											NUMBER(18) NOT NULL,
    SERIAL_NUMBER										VARCHAR2(124) NULL,
    OPERATOR											VARCHAR2(256) NULL,
    SMD_TYPE												VARCHAR2(256) NULL,
    SYSTEM_STATUS										VARCHAR2(256) NULL,
    LOCATION_CODE										VARCHAR2(256) NULL,
    LOCATION_DESCRIPTION									VARCHAR2(256) NULL,
    LOCATION_TYPE										VARCHAR2(256) NULL,
    LOCATION_GPS											VARCHAR2(256) NULL,
    LOCATION_ZONE_LIGHT									NUMBER(18) NOT NULL,
    LOCATION_ZONE_PT										NUMBER(18) NOT NULL,
    LOCATION_ZONE_HEAVY									NUMBER(18) NOT NULL,
    LOCATION_THRESHOLD_LIGHT								NUMBER(18) NOT NULL,
    LOCATION_THRESHOLD_PT									NUMBER(18) NOT NULL,
    LOCATION_THRESHOLD_HEAVY								NUMBER(18) NOT NULL,
    LAST_INFINGEMENT_TIME									VARCHAR2(256) NULL,
    LAST_INFINGEMENT_SPEED								NUMBER(18) NOT NULL,
    LAST_INFINGEMENT_DISTANCE								NUMBER(18) NOT NULL,
    LAST_INFINGEMENT_PLATE								VARCHAR2(256) NULL,
    LAST_INFINGEMENT_TYPE									VARCHAR2(256) NULL,
    LAST_VOSI_TIME										VARCHAR2(256) NULL,
    LAST_VOSI_PLATE										VARCHAR2(256) NULL,
    LAST_VOSI_REASON										VARCHAR2(256) NULL,
    SESSION_STATISTICS_UPTIME								VARCHAR2(256) NULL,
    SESSION_STATISTICS_VEHICLE_COUNT						NUMBER(18) NOT NULL,
    SESSION_STATISTICS_INFRINGEMENT_COUNT					NUMBER(18) NOT NULL,
    SESSION_STATISTICS_INFRINGEMENT_RATE					NUMBER(18, 2) NOT NULL,
    SESSION_STATISTICS_VEHICLE_HOURLY_RATE					NUMBER(18) NOT NULL,
    SESSION_STATISTICS_SPEED_INFRINGEMENT_COUNT				NUMBER(18) NOT NULL,
    SESSION_STATISTICS_REDLIGHT_INFRINGEMENT_COUNT			NUMBER(18) NOT NULL,
    SESSION_STATISTICS_HEADWAY_INFRINGEMENT_COUNT		    NUMBER(18) NOT NULL,
    SESSION_STATISTICS_STOPLINE_INFRINGEMENT_COUNT			NUMBER(18) NOT NULL,
    SESSION_STATISTICS_YELLOWLINE_INFRINGEMENT_COUNT		NUMBER(18) NOT NULL,
    SESSION_STATISTICS_LINEVIOLATION_COUNT					NUMBER(18) NOT NULL,
    SESSION_STATISTICS_EIGHTYFIVEPERCENTILE_SPEED			NUMBER(18) NOT NULL,
    SESSION_STATISTICS_AVERAGESPEED						NUMBER(18) NOT NULL,
    SESSION_STATISTICS_STANDARD_DEVIATION					NUMBER(18) NOT NULL,
    SESSION_STATISTICS_MAXIMUM_SPEED						NUMBER(18) NOT NULL,
    SESSION_STATISTICS_VOSI_COUNT							NUMBER(18) NOT NULL,
    DAY_STATISTICS_UPTIME									VARCHAR2(256) NULL,
    DAY_STATISTICS_VEHICLE_COUNT							NUMBER(18) NOT NULL,
    DAY_STATISTICS_INFRINGEMENT_COUNT						NUMBER(18) NOT NULL,
    DAY_STATISTICS_INFRINGEMENT_RATE						NUMBER(18, 2) NOT NULL,
    DAY_STATISTICS_VEHICLE_HOURLYRATE						NUMBER(18) NOT NULL,
    DAY_STATISTICS_SPEED_INFRINGEMENT_COUNT					NUMBER(18) NOT NULL,
    DAY_STATISTICS_REDLIGHT_INFRINGEMENT_COUNT				NUMBER(18) NOT NULL,
    DAY_STATISTICS_HEADWAY_INFRINGEMENT_COUNT			    NUMBER(18) NOT NULL,
    DAY_STATISTICS_STOPLINE_INFRINGEMENT_COUNT				NUMBER(18) NOT NULL,
    DAY_STATISTICS_YELLOWLINE_INFRINGEMENT_COUNT			NUMBER(18) NOT NULL,
    DAY_STATISTICS_LINEVIOLATION_COUNT						NUMBER(18) NOT NULL,
    DAY_STATISTICS_EIGHTYFIVEPERCENTILE_SPEED				NUMBER(18) NOT NULL,
    DAY_STATISTICS_AVERAGE_SPEED							NUMBER(18) NOT NULL,
    DAY_STATISTICS_STANDARD_DEVIATION						NUMBER(18) NOT NULL,
    DAY_STATISTICS_MAXIMUM_SPEED							NUMBER(18) NOT NULL,
    DAY_STATISTICS_VOSI_COUNT								NUMBER(18) NOT NULL,
	MODIFIED_TIMESTAMP										DATE NOT NULL
);
-- CREATE/RECREATE PRIMARY, UNIQUE AND FOREIGN KEY CONSTRAINTS 
ALTER TABLE CAMERA_LAST_STATISTICS ADD CONSTRAINT CAMERA_LAST_STATISTICS_PK PRIMARY KEY (ID) USING INDEX;

-- SEQUENCE
CREATE SEQUENCE  CAMERA_LAST_STATISTICS_SEQ;

-- TRIGGER
CREATE OR REPLACE TRIGGER CAMERA_LAST_STATISTICS_BI BEFORE INSERT ON CAMERA_LAST_STATISTICS FOR EACH ROW
BEGIN
	:NEW.ID := CAMERA_LAST_STATISTICS_SEQ.NEXTVAL;
END CAMERA_LAST_STATISTICS_BI;
/