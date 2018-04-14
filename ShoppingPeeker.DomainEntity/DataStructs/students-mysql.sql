/*
Navicat MySQL Data Transfer

Source Server         : localdb
Source Server Version : 50718
Source Host           : localhost:3306
Source Database       : demodb

Target Server Type    : MYSQL
Target Server Version : 50718
File Encoding         : 65001

Date: 2017-06-03 13:24:41
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for students
-- ----------------------------
DROP TABLE IF EXISTS `students`;
CREATE TABLE `students` (
  `Id` bigint(11) NOT NULL AUTO_INCREMENT,
  `Age` int(11) DEFAULT NULL,
  `Name` varchar(255) DEFAULT NULL,
  `Sex` tinyint(1) DEFAULT NULL,
  `AddTime` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `Score` decimal(7,2) DEFAULT NULL,
  `Longitude` double(15,6) DEFAULT NULL,
  `Latitude` double(15,6) DEFAULT NULL,
  `HasPay` float DEFAULT NULL,
  `HomeNumber` smallint(255) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `id-noclustered` (`Id`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of students
-- ----------------------------
INSERT INTO `students` VALUES ('1', '1', 'a', '1', '2017-06-03 11:28:10', '99.67', '5345367.666000', '333.000000', '44.3', '55');
INSERT INTO `students` VALUES ('2', '22', 'b', '0', '2017-06-03 11:28:12', '55.30', '55523.669966', '424343.000000', '88.6', '66');
INSERT INTO `students` VALUES ('3', '3', 'c', '1', '2017-06-03 11:28:14', '65.66', '77.220000', '34242.333000', '99.666', '77');
INSERT INTO `students` VALUES ('4', '23', 'd', '0', '2017-06-03 11:28:23', '44.15', '55.663360', '423424.660000', '55.33', '3333');
