BEGIN;
DROP SCHEMA IF EXISTS lab CASCADE;
CREATE SCHEMA lab;
CREATE TABLE lab.enrollments (
  tenant_id text NOT NULL,
  enrollment_id text NOT NULL,
  status text NOT NULL CHECK (status IN ('ACTIVE','SUSPENDED')),
  PRIMARY KEY (tenant_id, enrollment_id)
);
CREATE TABLE lab.partner_confirmations (
  confirmation_id integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  tenant_id text NOT NULL,
  batch_id integer NOT NULL,
  enrollment_id text NULL
);
INSERT INTO lab.enrollments (tenant_id, enrollment_id, status) VALUES
 ('A','E-101','ACTIVE'),('A','E-102','ACTIVE'),('A','E-103','ACTIVE'),
 ('A','E-104','SUSPENDED'),('A','E-105','ACTIVE'),('B','E-103','ACTIVE');
INSERT INTO lab.partner_confirmations (tenant_id,batch_id,enrollment_id) VALUES
 ('A',42,'E-101'),('A',42,'E-105'),('A',42,NULL),
 ('A',41,'E-102'),('B',42,'E-103');
COMMIT;
