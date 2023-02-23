-- MySQL Workbench Forward Engineering

SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';

-- -----------------------------------------------------
-- Schema mydb
-- -----------------------------------------------------
-- -----------------------------------------------------
-- Schema perla
-- -----------------------------------------------------

-- -----------------------------------------------------
-- Schema perla
-- -----------------------------------------------------
CREATE SCHEMA IF NOT EXISTS `perla` DEFAULT CHARACTER SET utf8 ;
USE `perla` ;

-- -----------------------------------------------------
-- Table `perla`.`customer`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `perla`.`customer` (
  `ID` DOUBLE NOT NULL,
  `Name` VARCHAR(100) NULL DEFAULT NULL,
  `PhoneNumber` DOUBLE NULL DEFAULT NULL,
  `MoneyPaid` DOUBLE NULL DEFAULT NULL,
  PRIMARY KEY (`ID`),
  UNIQUE INDEX `idCustomer_UNIQUE` (`ID` ASC) VISIBLE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb3;


-- -----------------------------------------------------
-- Table `perla`.`appointment`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `perla`.`appointment` (
  `ID` INT NOT NULL AUTO_INCREMENT,
  `Appointment_Data` DATETIME NOT NULL,
  `Customer_ID` DOUBLE NOT NULL,
  `Treatment` VARCHAR(100) NOT NULL,
  `MoneyPaid` DOUBLE NULL DEFAULT '0',
  PRIMARY KEY (`ID`),
  UNIQUE INDEX `idAppointment_UNIQUE` (`ID` ASC) VISIBLE,
  INDEX `Appointemnt_Treatment_idx` (`Treatment` ASC) VISIBLE,
  INDEX `Appointemnt_Customer_idx` (`Customer_ID` ASC) VISIBLE,
  CONSTRAINT `Appointemnt_Customer`
    FOREIGN KEY (`Customer_ID`)
    REFERENCES `perla`.`customer` (`ID`))
ENGINE = InnoDB
AUTO_INCREMENT = 55
DEFAULT CHARACTER SET = utf8mb3;


-- -----------------------------------------------------
-- Table `perla`.`spendinginfo`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `perla`.`spendinginfo` (
  `ID` INT NOT NULL AUTO_INCREMENT,
  `MoneySpent` DOUBLE NOT NULL,
  `Date` DATE NOT NULL,
  `Description` LONGTEXT NULL DEFAULT NULL,
  PRIMARY KEY (`ID`))
ENGINE = InnoDB
AUTO_INCREMENT = 13
DEFAULT CHARACTER SET = utf8mb3;


SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
