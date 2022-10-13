CREATE TABLE viot.users (
  userid bigint NOT NULL AUTO_INCREMENT,
  username varchar(255) NOT NULL,
  pwdhash varchar(255) NOT NULL,
  phone varchar(20) NOT NULL,
  flags bigint DEFAULT NULL,
  firstname varchar(255) DEFAULT NULL,
  lastname varchar(255) DEFAULT NULL,
  country char(3) NOT NULL DEFAULT 'ISR',
  region varchar(150) DEFAULT NULL,
  city varchar(150) DEFAULT NULL,
  street varchar(200) DEFAULT NULL,
  user_type int NOT NULL DEFAULT 0,
  regdate datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  email varchar(255) DEFAULT NULL,
  providerid bigint NOT NULL,
  log_level int DEFAULT 1,
  enddate datetime DEFAULT NULL,
  culture_id char(5) NOT NULL DEFAULT 'en-US',
  auth_token varchar(200) DEFAULT NULL,
  PRIMARY KEY (userid)
)
ENGINE = INNODB,
AUTO_INCREMENT = 100152,
AVG_ROW_LENGTH = 2730,
CHARACTER SET utf8,
COLLATE utf8_general_ci;

ALTER TABLE viot.users
ADD INDEX IX_users_email (email);

ALTER TABLE viot.users
ADD INDEX IX_users_regdate (regdate);

ALTER TABLE viot.users
ADD UNIQUE INDEX UQ_users_username (username, providerid);




CREATE TABLE viot.user_tokens (
  tokenid bigint NOT NULL,
  userid bigint NOT NULL,
  targetid varchar(255) NOT NULL,
  token_type int NOT NULL,
  token varchar(255) NOT NULL,
  os int NOT NULL DEFAULT 0,
  expired_date datetime DEFAULT NULL,
  regdate datetime NOT NULL,
  os_version varchar(100) DEFAULT NULL,
  last_action datetime DEFAULT NULL,
  PRIMARY KEY (tokenid)
)
ENGINE = INNODB,
AVG_ROW_LENGTH = 5461,
CHARACTER SET utf8,
COLLATE utf8_general_ci;

ALTER TABLE viot.user_tokens
ADD UNIQUE INDEX UQ_user_tokens_u_tt_tid (userid, token_type, targetid);

ALTER TABLE viot.user_tokens
ADD CONSTRAINT FK_user_tokens_users FOREIGN KEY (userid)
REFERENCES viot.users (userid) ON DELETE CASCADE;





CREATE TABLE viot.providers (
  providerid bigint UNSIGNED NOT NULL,
  provider_name varchar(100) NOT NULL,
  provider_desc varchar(500) DEFAULT NULL,
  flags int NOT NULL DEFAULT 1,
  parent_providerid bigint UNSIGNED DEFAULT NULL,
  cultureid varchar(10) DEFAULT NULL,
  alias varchar(100) NOT NULL,
  PRIMARY KEY (providerid)
)
ENGINE = INNODB,
AVG_ROW_LENGTH = 1092,
CHARACTER SET utf8,
COLLATE utf8_general_ci;

ALTER TABLE viot.providers
ADD UNIQUE INDEX alias (alias);

ALTER TABLE viot.providers
ADD UNIQUE INDEX UQ_providers_name (provider_name);

ALTER TABLE viot.providers
ADD CONSTRAINT providers_FK FOREIGN KEY (parent_providerid)
REFERENCES viot.providers (providerid);





CREATE TABLE viot.device_types (
  device_type_id bigint NOT NULL AUTO_INCREMENT,
  device_type_name varchar(255) NOT NULL,
  PRIMARY KEY (device_type_id)
)
ENGINE = INNODB,
AUTO_INCREMENT = 11,
AVG_ROW_LENGTH = 16384,
CHARACTER SET utf8,
COLLATE utf8_general_ci;




CREATE TABLE viot.device_manufactors (
  manufactorid bigint NOT NULL AUTO_INCREMENT,
  manufactor_name varchar(255) NOT NULL,
  regdate datetime NOT NULL,
  PRIMARY KEY (manufactorid)
)
ENGINE = INNODB,
AUTO_INCREMENT = 11,
AVG_ROW_LENGTH = 8192,
CHARACTER SET utf8,
COLLATE utf8_general_ci;




CREATE TABLE viot.devices (
  deviceid bigint NOT NULL AUTO_INCREMENT,
  name varchar(100) DEFAULT NULL,
  status int NOT NULL DEFAULT 0,
  device_type_id bigint NOT NULL,
  providerid bigint UNSIGNED NOT NULL DEFAULT 1,
  latitude decimal(9, 6) DEFAULT NULL,
  longitude decimal(9, 6) DEFAULT NULL,
  location varchar(500) DEFAULT NULL,
  serial_number varchar(120) NOT NULL,
  mac_address varchar(100) NOT NULL,
  model_name varchar(150) DEFAULT NULL,
  hardware_version varchar(150) DEFAULT NULL,
  firmware_version varchar(150) DEFAULT NULL,
  regdate datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  userid bigint NOT NULL,
  wifi_mac varchar(255) DEFAULT NULL,
  manufactorid bigint NOT NULL,
  iconid varchar(32) DEFAULT NULL,
  has_image tinyint(1) DEFAULT NULL,
  token varchar(100) DEFAULT NULL,
  mqtt_clientid varchar(255) DEFAULT NULL,
  log_level int DEFAULT NULL,
  enable_events int NOT NULL DEFAULT 0,
  is_activated tinyint(1) NOT NULL DEFAULT 0,
  last_int_activity datetime DEFAULT NULL,
  power_on_id char(32) DEFAULT NULL,
  PRIMARY KEY (deviceid)
)
ENGINE = INNODB,
AUTO_INCREMENT = 205525,
AVG_ROW_LENGTH = 4096,
CHARACTER SET utf8,
COLLATE utf8_general_ci;

ALTER TABLE viot.devices
ADD INDEX devices_lastintact (last_int_activity);

ALTER TABLE viot.devices
ADD UNIQUE INDEX UK_devices_mac_address (mac_address);

ALTER TABLE viot.devices
ADD UNIQUE INDEX UQ_devices_sn (serial_number);

ALTER TABLE viot.devices
ADD CONSTRAINT devices_FK FOREIGN KEY (providerid)
REFERENCES viot.providers (providerid);

ALTER TABLE viot.devices
ADD CONSTRAINT FK_devices_dmanuactors FOREIGN KEY (manufactorid)
REFERENCES viot.device_manufactors (manufactorid);

ALTER TABLE viot.devices
ADD CONSTRAINT FK_devices_dtypes FOREIGN KEY (device_type_id)
REFERENCES viot.device_types (device_type_id) ON DELETE CASCADE;

ALTER TABLE viot.devices
ADD CONSTRAINT FK_devices_users FOREIGN KEY (userid)
REFERENCES viot.users (userid) ON DELETE CASCADE;




CREATE TABLE viot.sec_objs (
  objid char(32) NOT NULL,
  obj_name varchar(100) NOT NULL,
  obj_desc varchar(255) DEFAULT NULL,
  PRIMARY KEY (objid)
)
ENGINE = INNODB,
AVG_ROW_LENGTH = 2730,
CHARACTER SET utf8,
COLLATE utf8_general_ci;

ALTER TABLE viot.sec_objs
ADD UNIQUE INDEX UK_sec_objs_obj_name (obj_name);




CREATE TABLE viot.sec_role_permissions (
  permid char(32) NOT NULL,
  roleid char(32) NOT NULL,
  objid char(32) NOT NULL,
  perms int NOT NULL,
  PRIMARY KEY (permid)
)
ENGINE = INNODB,
AVG_ROW_LENGTH = 910,
CHARACTER SET utf8,
COLLATE utf8_general_ci;

ALTER TABLE viot.sec_role_permissions
ADD UNIQUE INDEX uq_sec_role_permissions (roleid, objid);

ALTER TABLE viot.sec_role_permissions
ADD CONSTRAINT FK_sec_role_perm_o FOREIGN KEY (objid)
REFERENCES viot.sec_objs (objid) ON DELETE CASCADE ON UPDATE CASCADE;

ALTER TABLE viot.sec_role_permissions
ADD CONSTRAINT FK_sec_role_perm_r FOREIGN KEY (roleid)
REFERENCES viot.sec_roles (roleid) ON DELETE CASCADE ON UPDATE CASCADE;





CREATE TABLE viot.sec_roles (
  roleid char(32) NOT NULL,
  role_name varchar(50) NOT NULL,
  role_desc varchar(255) DEFAULT NULL,
  providerid bigint UNSIGNED DEFAULT NULL,
  PRIMARY KEY (roleid)
)
ENGINE = INNODB,
AVG_ROW_LENGTH = 5461,
CHARACTER SET utf8,
COLLATE utf8_general_ci;

ALTER TABLE viot.sec_roles
ADD UNIQUE INDEX UK_sec_roles_role_name (role_name);




CREATE TABLE viot.sec_user_roles (
  urid char(32) NOT NULL,
  userid bigint NOT NULL,
  roleid char(32) NOT NULL,
  PRIMARY KEY (urid)
)
ENGINE = INNODB,
AVG_ROW_LENGTH = 8192,
CHARACTER SET utf8,
COLLATE utf8_general_ci;

ALTER TABLE viot.sec_user_roles
ADD UNIQUE INDEX uq_sec_user_roles (userid, roleid);

ALTER TABLE viot.sec_user_roles
ADD CONSTRAINT FK_user_roles_r FOREIGN KEY (roleid)
REFERENCES viot.sec_roles (roleid) ON DELETE CASCADE ON UPDATE CASCADE;

ALTER TABLE viot.sec_user_roles
ADD CONSTRAINT FK_user_roles_u FOREIGN KEY (userid)
REFERENCES viot.users (userid) ON DELETE CASCADE ON UPDATE CASCADE;


