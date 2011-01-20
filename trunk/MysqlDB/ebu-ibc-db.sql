CREATE DATABASE  IF NOT EXISTS `ebuplayout-dev2` /*!40100 DEFAULT CHARACTER SET latin1 */;
USE `ebuplayout-dev2`;
-- MySQL dump 10.13  Distrib 5.1.40, for Win32 (ia32)
--
-- Host: localhost    Database: ebuplayout-dev2
-- ------------------------------------------------------
-- Server version	5.5.8

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `canvasitem`
--

DROP TABLE IF EXISTS `canvasitem`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `canvasitem` (
  `idcanvasitem` int(11) NOT NULL AUTO_INCREMENT,
  `idcanvasslice` int(11) NOT NULL,
  `clockPosition` int(11) DEFAULT NULL,
  `type` varchar(45) DEFAULT NULL,
  `param1` varchar(45) DEFAULT NULL,
  `param2` varchar(45) DEFAULT NULL,
  `param3` varchar(45) DEFAULT NULL,
  `label` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`idcanvasitem`),
  UNIQUE KEY `idcanvasItem_UNIQUE` (`idcanvasitem`)
) ENGINE=MyISAM AUTO_INCREMENT=80 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `canvasitem`
--

LOCK TABLES `canvasitem` WRITE;
/*!40000 ALTER TABLE `canvasitem` DISABLE KEYS */;
INSERT INTO `canvasitem` VALUES (40,13,2,'SPECIFICITEM','580','','','IBC 2010 - Top Horaire'),(41,13,4,'SPECIFICITEM','581','','','IN - News'),(42,13,8,'SPECIFICITEM','582','','','OUT - News'),(43,13,6,'CATEGORYITEM','NWS','','','News'),(44,13,0,'SYNC','HARD','00:00:00','','SYNC HARD : 00:00:00'),(45,13,10,'CATEGORYITEM','A','','','Rotation A'),(46,13,13,'CATEGORYITEM','B','','','Rotation B'),(47,13,11,'CATEGORYITEM','C','','','Rotation C'),(48,13,11,'CATEGORYITEM','J','','','Jingles'),(49,13,12,'CATEGORYITEM','J','','','Jingles'),(63,13,19,'SLIDESLOAD','newstitle,newstitle,newstitle','','','SLIDES CHANGE : newstitle,newstitle,newstitle'),(53,13,14,'CATEGORYITEM','J','','','Jingles'),(54,13,15,'CATEGORYITEM','D1','','','Fill Primary'),(55,13,17,'CATEGORYITEM','D2','','','Fill Secondary'),(56,13,16,'CATEGORYITEM','J','','','Jingles'),(57,13,18,'CATEGORYITEM','N','','','New'),(58,13,9,'SLIDESLOAD','ebu,traffic,song','','','SLIDES CHANGE : ebu,traffic,song'),(59,13,1,'SLIDESLOAD','ebuclock,ebuclock,ebuclock','','','SLIDES CHANGE : ebuclock,ebuclock,ebuclock'),(60,13,3,'SLIDESLOAD','newstitle,newstitle,newstitle','','','SLIDES CHANGE : newstitle,newstitle,newstitle'),(61,13,5,'SLIDESLOAD','news1,news2,news3,news4','','','SLIDES CHANGE : news1,news2,news3,news4'),(62,13,7,'SLIDESLOAD','newsoff,newsoff,newsoff','','','SLIDES CHANGE : newsoff,newsoff,newsoff'),(64,13,20,'SPECIFICITEM','581','','','IN - News'),(65,13,21,'SLIDESLOAD','news1,news2,news3,news4','','','SLIDES CHANGE : news1,news2,news3,news4'),(66,13,22,'CATEGORYITEM','NWS','','','News'),(67,13,23,'SLIDESLOAD','newsoff,newsoff,newsoff','','','SLIDES CHANGE : newsoff,newsoff,newsoff'),(68,13,24,'SPECIFICITEM','582','','','OUT - News'),(69,13,25,'SLIDESLOAD','ebu,traffic,song','','','SLIDES CHANGE : ebu,traffic,song'),(70,13,26,'CATEGORYITEM','A','','','Rotation A'),(71,13,27,'CATEGORYITEM','J','','','Jingles'),(72,13,28,'CATEGORYITEM','C','','','Rotation C'),(73,13,29,'CATEGORYITEM','J','','','Jingles'),(74,13,30,'CATEGORYITEM','B','','','Rotation B'),(75,13,31,'CATEGORYITEM','J','','','Jingles'),(76,13,32,'CATEGORYITEM','D1','','','Fill Primary'),(77,13,33,'CATEGORYITEM','J','','','Jingles'),(78,13,34,'CATEGORYITEM','D2','','','Fill Secondary'),(79,13,35,'CATEGORYITEM','N','','','New');
/*!40000 ALTER TABLE `canvasitem` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `dataitems_category`
--

DROP TABLE IF EXISTS `dataitems_category`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `dataitems_category` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(45) DEFAULT NULL,
  `shortname` varchar(5) DEFAULT NULL,
  `type` varchar(45) NOT NULL DEFAULT 'MUSIC',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `dataitems_category`
--

LOCK TABLES `dataitems_category` WRITE;
/*!40000 ALTER TABLE `dataitems_category` DISABLE KEYS */;
INSERT INTO `dataitems_category` VALUES (1,'Rotation B','B','MUSIC'),(2,'New','N','MUSIC'),(3,'Rotation A','A','MUSIC'),(4,'Fill Primary','D1','MUSIC'),(5,'Fill Secondary','D2','MUSIC'),(6,'Top Horaire','T','MISC'),(7,'Jingles News','JN','MISC'),(8,'News','NWS','NEWS'),(9,'Promos','P','COMMERCIAL'),(10,'Jingles','J','MISC'),(11,'Rotation C','C','MUSIC');
/*!40000 ALTER TABLE `dataitems_category` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `canvasslice`
--

DROP TABLE IF EXISTS `canvasslice`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `canvasslice` (
  `idcanvasslice` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(45) DEFAULT NULL,
  `synctype` varchar(45) DEFAULT 'NONE',
  `syncparam` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`idcanvasslice`)
) ENGINE=MyISAM AUTO_INCREMENT=14 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `canvasslice`
--

LOCK TABLES `canvasslice` WRITE;
/*!40000 ALTER TABLE `canvasslice` DISABLE KEYS */;
INSERT INTO `canvasslice` VALUES (13,'NORMAL CLOCK','NONE',NULL);
/*!40000 ALTER TABLE `canvasslice` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `scheduledslices`
--

DROP TABLE IF EXISTS `scheduledslices`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `scheduledslices` (
  `idscheduled` int(10) NOT NULL AUTO_INCREMENT,
  `scheduleddatetime` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`idscheduled`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `scheduledslices`
--

LOCK TABLES `scheduledslices` WRITE;
/*!40000 ALTER TABLE `scheduledslices` DISABLE KEYS */;
/*!40000 ALTER TABLE `scheduledslices` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `scheduledslots`
--

DROP TABLE IF EXISTS `scheduledslots`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `scheduledslots` (
  `idscheduledslots` int(11) NOT NULL AUTO_INCREMENT,
  `idscheduledslice` int(11) NOT NULL,
  `position` int(11) NOT NULL DEFAULT '0',
  `type` varchar(45) NOT NULL,
  `iddataitem` int(11) DEFAULT NULL,
  `param` varchar(255) DEFAULT NULL,
  `label` varchar(255) DEFAULT NULL,
  `scheduleddatetime` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`idscheduledslots`)
) ENGINE=MyISAM AUTO_INCREMENT=6275 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `scheduledslots`
--

LOCK TABLES `scheduledslots` WRITE;
/*!40000 ALTER TABLE `scheduledslots` DISABLE KEYS */;
/*!40000 ALTER TABLE `scheduledslots` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `canvasperiod`
--

DROP TABLE IF EXISTS `canvasperiod`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `canvasperiod` (
  `idcanvasperiod` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(45) DEFAULT NULL,
  `size` int(11) DEFAULT NULL COMMENT 'in hours',
  PRIMARY KEY (`idcanvasperiod`)
) ENGINE=MyISAM AUTO_INCREMENT=3 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `canvasperiod`
--

LOCK TABLES `canvasperiod` WRITE;
/*!40000 ALTER TABLE `canvasperiod` DISABLE KEYS */;
INSERT INTO `canvasperiod` VALUES (1,'NORMAL DAY',24);
/*!40000 ALTER TABLE `canvasperiod` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `canvasperiod_items`
--

DROP TABLE IF EXISTS `canvasperiod_items`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `canvasperiod_items` (
  `idcanvasperiod_items` int(11) NOT NULL AUTO_INCREMENT,
  `idperiod` int(11) NOT NULL,
  `idslice` int(11) NOT NULL,
  `position` int(11) DEFAULT NULL,
  PRIMARY KEY (`idcanvasperiod_items`)
) ENGINE=MyISAM AUTO_INCREMENT=33 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `canvasperiod_items`
--

LOCK TABLES `canvasperiod_items` WRITE;
/*!40000 ALTER TABLE `canvasperiod_items` DISABLE KEYS */;
INSERT INTO `canvasperiod_items` VALUES (23,1,13,0),(24,1,13,1),(25,1,13,2),(26,1,13,3),(27,1,13,4),(28,1,13,5),(29,1,13,6),(30,1,13,7),(31,1,13,8),(32,1,13,9);
/*!40000 ALTER TABLE `canvasperiod_items` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `dataitems`
--

DROP TABLE IF EXISTS `dataitems`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `dataitems` (
  `iddataitems` int(11) NOT NULL AUTO_INCREMENT,
  `type` varchar(45) DEFAULT 'NONAUDIO',
  `label` varchar(255) DEFAULT NULL,
  `runtime` int(11) DEFAULT '0' COMMENT 'in ms',
  `title` varchar(255) DEFAULT NULL,
  `artist` varchar(255) DEFAULT NULL,
  `file` varchar(255) DEFAULT NULL,
  `radiovis1` varchar(255) NOT NULL,
  `radiovis2` varchar(255) NOT NULL,
  `radiovis3` varchar(255) NOT NULL,
  `radiovis4` varchar(255) NOT NULL,
  `radiovistxt` varchar(255) NOT NULL,
  `tmcue` int(11) DEFAULT '0' COMMENT 'in ms',
  `tmnext` int(11) DEFAULT '-1' COMMENT 'in ms',
  `category` varchar(25) DEFAULT NULL,
  PRIMARY KEY (`iddataitems`)
) ENGINE=MyISAM AUTO_INCREMENT=583 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `dataitems`
--

LOCK TABLES `dataitems` WRITE;
/*!40000 ALTER TABLE `dataitems` DISABLE KEYS */;
INSERT INTO `dataitems` VALUES (476,'AUDIOFILE','Pink - Funhouse',205000,'Funhouse','Pink','1-07 Funhouse.mp3','115.jpg','0','0','0','0',0,0,'B'),(475,'AUDIOFILE','Phantom Planet - California',194000,'California','Phantom Planet','2-13 California.mp3','114.jpg','0','0','0','0',0,0,'D1'),(474,'AUDIOFILE','Peter Fox - Haus Am See',216000,'Haus Am See','Peter Fox','1-20 Haus Am See.mp3','113.jpg','0','0','0','0',0,0,'C'),(473,'AUDIOFILE','Peter Cincotti - Goodbye Philadelphia',225000,'Goodbye Philadelphia','Peter Cincotti','Goodby Philadelphia.mp3','112.jpg','0','0','0','0',0,0,'D1'),(472,'AUDIOFILE','Paolo Nutini - Last Request',219000,'Last Request','Paolo Nutini','08 Last Request.mp3','111.jpg','0','0','0','0',0,0,'C'),(471,'AUDIOFILE','OneRepublic - Secrets',225000,'Secrets','OneRepublic','2-02 Secrets.mp3','110.jpg','0','0','0','0',0,0,'B'),(470,'AUDIOFILE','OneRepublic - Stop and stare',223000,'Stop and stare','OneRepublic','Stop and stare.mp3','109.jpg','0','0','0','0',0,0,'D1'),(469,'AUDIOFILE','The Offspring - Kristy, Are You Doing Okay?',221000,'Kristy, Are You Doing Okay?','The Offspring','01 Kristy, Are You Doing Okay_.mp3','108.jpg','0','0','0','0',0,0,'B'),(468,'AUDIOFILE','Nickelback - Rockstar',254000,'Rockstar','Nickelback','2-20 Rockstar.mp3','107.jpg','0','0','0','0',0,0,'B'),(467,'AUDIOFILE','Nickelback - If everyone cared',216000,'If everyone cared','Nickelback','12 If everyone cared.mp3','106.jpg','0','0','0','0',0,0,'C'),(466,'AUDIOFILE','Nick Lachey - What´s Left Of Me',246000,'What´s Left Of Me','Nick Lachey','13 What_s Left Of Me.mp3','105.jpg','0','0','0','0',0,0,'C'),(465,'AUDIOFILE','Nena - Liebe Ist',243000,'Liebe Ist','Nena','2-17 Liebe Ist.mp3','104.jpg','0','0','0','0',0,0,'D1'),(464,'AUDIOFILE','Nelly Furtado - Maneater',266000,'Maneater','Nelly Furtado','09 Maneater.mp3','103.jpg','0','0','0','0',0,0,'C'),(463,'AUDIOFILE','Nelly Furtado - All good Things (come to an end)',261000,'All good Things (come to an end)','Nelly Furtado','05 All good Things (come to an end).mp3','102.jpg','0','0','0','0',0,0,'C'),(462,'AUDIOFILE','Nelly Furtado - Say it right',214000,'Say it right','Nelly Furtado','Say it right.mp3','101.jpg','0','0','0','0',0,0,'D1'),(461,'AUDIOFILE','Milow - Ayo Technology',213000,'Ayo Technology','Milow','01 Ayo Technology.mp3','100.jpg','0','0','0','0',0,0,'A'),(460,'AUDIOFILE','Milow - You Don\'t Know',168000,'You Don\'t Know','Milow','2-01 You Don\'t Know.mp3','99.jpg','0','0','0','0',0,0,'B'),(458,'AUDIOFILE','Mika - Grace Kelly',187000,'Grace Kelly','Mika','Grace Kelly.mp3','97.jpg','0','0','0','0',0,0,'D1'),(459,'AUDIOFILE','Mika - Relax take it easy',224000,'Relax take it easy','Mika','Relax take it easy.mp3','98.jpg','0','0','0','0',0,0,'D2'),(457,'AUDIOFILE','Mika - Big girl',189000,'Big girl','Mika','Big girl.mp3','96.jpg','0','0','0','0',0,0,'D2'),(455,'AUDIOFILE','MGMT - Kids',302000,'Kids','MGMT','1-07 Kids.mp3','94.jpg','0','0','0','0',0,0,'B'),(456,'AUDIOFILE','Mika - We Are Golden',238000,'We Are Golden','Mika','1-11 We Are Golden.mp3','95.jpg','0','0','0','0',0,0,'N'),(454,'AUDIOFILE','Metro Station - Shake It',182000,'Shake It','Metro Station','2-21 Shake It.mp3','93.jpg','0','0','0','0',0,0,'B'),(453,'AUDIOFILE','Melanie C - First Day Of My Life',242000,'First Day Of My Life','Melanie C','1-01 First Day Of My Life.mp3','92.jpg','0','0','0','0',0,0,'D1'),(452,'AUDIOFILE','Marquess - Vayamos companeros',180000,'Vayamos companeros','Marquess','Vayamos companeros.mp3','91.jpg','0','0','0','0',0,0,'D2'),(451,'AUDIOFILE','Maroon 5 - Makes me wonder',211000,'Makes me wonder','Maroon 5','Makes me wonder.mp3','90.jpg','0','0','0','0',0,0,'D1'),(450,'AUDIOFILE','Mark Ronson feat. Amy Winehouse - Valerie',219000,'Valerie','Mark Ronson feat. Amy Winehouse','Valerie.mp3','89.jpg','0','0','0','0',0,0,'D1'),(449,'AUDIOFILE','Marit Larsen - If A Song Could Get Me You',202000,'If A Song Could Get Me You','Marit Larsen','1-03 If A Song Could Get Me You.mp3','88.jpg','0','0','0','0',0,0,'A'),(448,'AUDIOFILE','Mando Diao - Dance With Somebody',240000,'Dance With Somebody','Mando Diao','1-01 Dance With Somebody.mp3','87.jpg','0','0','0','0',0,0,'B'),(447,'AUDIOFILE','Madonna - Celebration',215000,'Celebration','Madonna','01 Celebration.mp3','86.jpg','0','0','0','0',0,0,'B'),(446,'AUDIOFILE','Madonna - Jump',202000,'Jump','Madonna','Jump.mp3','85.jpg','0','0','0','0',0,0,'D1'),(445,'AUDIOFILE','Linkin Park - Shadow of The Day',289000,'Shadow of The Day','Linkin Park','Shadow of The Day.mp3','84.jpg','0','0','0','0',0,0,'D1'),(444,'AUDIOFILE','Lily Allen - Smile',196000,'Smile','Lily Allen','11 Smile.mp3','83.jpg','0','0','0','0',0,0,'C'),(443,'AUDIOFILE','Lily Allen - Not Fair',201000,'Not Fair','Lily Allen','03 Not Fair.mp3','82.jpg','0','0','0','0',0,0,'B'),(442,'AUDIOFILE','Leona Lewis - Bleeding Love',263000,'Bleeding Love','Leona Lewis','Bleeding Love.mp3','81.jpg','0','0','0','0',0,0,'D1'),(441,'AUDIOFILE','Lenka - The Show',235000,'The Show','Lenka','01 The Show.mp3','80.jpg','0','0','0','0',0,0,'A'),(440,'AUDIOFILE','Laura Pausini & James Blunt - Primavera In Anticipo (It Is My Song)',209000,'Primavera In Anticipo (It Is My Song)','Laura Pausini & James Blunt','2-08 Primavera In Anticipo (It Is My.mp3','79.jpg','0','0','0','0',0,0,'B'),(439,'AUDIOFILE','Lady GaGa - Poker Face',237000,'Poker Face','Lady GaGa','01 Poker Face.mp3','78.jpg','0','0','0','0',0,0,'A'),(438,'AUDIOFILE','La Roux - Bulletproof',205000,'Bulletproof','La Roux','04 Bulletproof.mp3','77.jpg','0','0','0','0',0,0,'B'),(437,'AUDIOFILE','Kooks - Shine On',193000,'Shine On','Kooks','1-12 Shine On.mp3','76.jpg','0','0','0','0',0,0,'B'),(436,'AUDIOFILE','Kooks - She moves in her own way',169000,'She moves in her own way','Kooks','She moves in her own way.mp3','75.jpg','0','0','0','0',0,0,'D2'),(435,'AUDIOFILE','Kings Of Leon - Use Somebody',231000,'Use Somebody','Kings Of Leon','01 Use Somebody.mp3','74.jpg','0','0','0','0',0,0,'C'),(434,'AUDIOFILE','Kim Wilde - You Came (2006 Version)',190000,'You Came (2006 Version)','Kim Wilde','12 You Came (2006 Version).mp3','73.jpg','0','0','0','0',0,0,'C'),(433,'AUDIOFILE','The Killers - Human',244000,'Human','The Killers','1-04 Human.mp3','72.jpg','0','0','0','0',0,0,'A'),(432,'AUDIOFILE','Kid Rock - All Summer Long',296000,'All Summer Long','Kid Rock','All Summer Long.mp3','71.jpg','0','0','0','0',0,0,'D1'),(431,'AUDIOFILE','Kelly Clarkson - Breakaway',237000,'Breakaway','Kelly Clarkson','16 Breakaway.mp3','70.jpg','0','0','0','0',0,0,'C'),(430,'AUDIOFILE','Kelly Clarkson - Walk away',187000,'Walk away','Kelly Clarkson','09 Walk away.mp3','69.jpg','0','0','0','0',0,0,'C'),(429,'AUDIOFILE','Kelly Clarkson - Because Of You',220000,'Because Of You','Kelly Clarkson','02 Because Of You.mp3','68.jpg','0','0','0','0',0,0,'C'),(428,'AUDIOFILE','Kelly Clarkson - My Life Would Suck Without You',211000,'My Life Would Suck Without You','Kelly Clarkson','01 My Life Would Suck Without You.mp3','67.jpg','0','0','0','0',0,0,'B'),(427,'AUDIOFILE','Katy Perry - Hot N Cold',221000,'Hot N Cold','Katy Perry','1-08 Hot N Cold.mp3','66.jpg','0','0','0','0',0,0,'C'),(426,'AUDIOFILE','Katie Melva - Nine Million Bicycles',194000,'Nine Million Bicycles','Katie Melva','18 Nine Million Bicycles.mp3','65.jpg','0','0','0','0',0,0,'C'),(425,'AUDIOFILE','Katy Perry - I kissed a girl',181000,'I kissed a girl','Katy Perry','I kissed a girl.mp3','64.jpg','0','0','0','0',0,0,'D1'),(424,'AUDIOFILE','Kaiser Chiefs - Ruby',203000,'Ruby','Kaiser Chiefs','06 Ruby.mp3','63.jpg','0','0','0','0',0,0,'C'),(423,'AUDIOFILE','K-Maro - Femme Like U',239000,'Femme Like U','K-Maro','1-14 Femme Like U.mp3','62.jpg','0','0','0','0',0,0,'D1'),(422,'AUDIOFILE','Justin Timberlake - What goes around comes around',211000,'What goes around comes around','Justin Timberlake','What goes around comes around.mp3','61.jpg','0','0','0','0',0,0,'D2'),(421,'AUDIOFILE','Juli - Dieses Leben',233000,'Dieses Leben','Juli','07 Dieses Leben.mp3','60.jpg','0','0','0','0',0,0,'C'),(420,'AUDIOFILE','Juanes - La Camisa Negra',215000,'La Camisa Negra','Juanes','1-12 La Camisa Negra.mp3','59.jpg','0','0','0','0',0,0,'D1'),(419,'AUDIOFILE','Juanes - A dios le pido',205000,'A dios le pido','Juanes','A dios le pido.mp3','58.jpg','0','0','0','0',0,0,'D1'),(418,'AUDIOFILE','Joana Zimmer - I Believe',251000,'I Believe','Joana Zimmer','2-09 I Believe.mp3','57.jpg','0','0','0','0',0,0,'D1'),(417,'AUDIOFILE','James Morrison Feat. Nelly Furtado - Broken Strings',252000,'Broken Strings','James Morrison Feat. Nelly Furtado','2-16 Broken Strings.mp3','56.jpg','0','0','0','0',0,0,'B'),(416,'AUDIOFILE','James Morrison - Wonderfull world',205000,'Wonderfull world','James Morrison','16 Wonderfull world.mp3','55.jpg','0','0','0','0',0,0,'C'),(415,'AUDIOFILE','James Morrison - You Give Me Something',211000,'You Give Me Something','James Morrison','04 You Give Me Something.mp3','54.jpg','0','0','0','0',0,0,'C'),(414,'AUDIOFILE','James Morrison - Please Don\'t Stop The Rain',235000,'Please Don\'t Stop The Rain','James Morrison','2-05 Please Don\'t Stop The Rain 1.mp3','53.jpg','0','0','0','0',0,0,'B'),(413,'AUDIOFILE','James Blunt - Wisemen',222000,'Wisemen','James Blunt','17 Wisemen.mp3','52.jpg','0','0','0','0',0,0,'C'),(412,'AUDIOFILE','James Blunt - 1973',240000,'1973','James Blunt','1973.mp3','51.jpg','0','0','0','0',0,0,'D1'),(411,'AUDIOFILE','James Blunt - Same Mistake',234000,'Same Mistake','James Blunt','Same Mistake.mp3','50.jpg','0','0','0','0',0,0,'D2'),(410,'AUDIOFILE','Jack Johnson - Sitting waiting wishing',187000,'Sitting waiting wishing','Jack Johnson','Sitting waiting wishing.mp3','49.jpg','0','0','0','0',0,0,'D2'),(409,'AUDIOFILE','Ich + Ich - Du Erinnerst Mich An Liebe',211000,'Du Erinnerst Mich An Liebe','Ich + Ich','2-03 Du Erinnerst Mich An Liebe.mp3','48.jpg','0','0','0','0',0,0,'D1'),(408,'AUDIOFILE','Ich + Ich - Vom selben Stern',226000,'Vom selben Stern','Ich + Ich','Vom selben Stern.mp3','47.jpg','0','0','0','0',0,0,'D2'),(407,'AUDIOFILE','Ich + Ich - Stark',222000,'Stark','Ich + Ich','Stark.mp3','46.jpg','0','0','0','0',0,0,'D1'),(406,'AUDIOFILE','Ich + Ich - So Soll Es Bleiben',195000,'So Soll Es Bleiben','Ich + Ich','So Soll Es Bleiben.mp3','45.jpg','0','0','0','0',0,0,'D2'),(405,'AUDIOFILE','Ich + Ich - Pflaster',205000,'Pflaster','Ich + Ich','2-01 Pflaster.mp3','44.jpg','0','0','0','0',0,0,'A'),(404,'AUDIOFILE','Herbert Grönemeyer - Lied 1 - Stück vom Himmel',277000,'Lied 1 - Stück vom Himmel','Herbert Grönemeyer','20 Lied 1 - Stück vom Himmel.mp3','43.jpg','0','0','0','0',0,0,'C'),(403,'AUDIOFILE','Gwen Stefani - The Sweet Escape',241000,'The Sweet Escape','Gwen Stefani','The Sweet Escape.mp3','42.jpg','0','0','0','0',0,0,'D1'),(402,'AUDIOFILE','Green Day - 21 Guns',321000,'21 Guns','Green Day','16 21 Guns.mp3','41.jpg','0','0','0','0',0,0,'B'),(401,'AUDIOFILE','Green Day - Know Your Enemy',191000,'Know Your Enemy','Green Day','01 Know Your Enemy.mp3','40.jpg','0','0','0','0',0,0,'B'),(400,'AUDIOFILE','Green Day - Wake me up when september ends',199000,'Wake me up when september ends','Green Day','Wake me up when september ends.mp3','39.jpg','0','0','0','0',0,0,'D1'),(399,'AUDIOFILE','The Gossip - Heavy Cross',242000,'Heavy Cross','The Gossip','01 Heavy Cross.mp3','38.jpg','0','0','0','0',0,0,'B'),(398,'AUDIOFILE','Gnarls Barkley - Crazy',177000,'Crazy','Gnarls Barkley','Crazy.mp3','37.jpg','0','0','0','0',0,0,'D2'),(397,'AUDIOFILE','Gavin DeGraw - Chariot',231000,'Chariot','Gavin DeGraw','17 Chariot.mp3','36.jpg','0','0','0','0',0,0,'C'),(396,'AUDIOFILE','Gabriella Cilmi - Sweet about me',201000,'Sweet about me','Gabriella Cilmi','Sweet about me.mp3','35.jpg','0','0','0','0',0,0,'D1'),(395,'AUDIOFILE','Franz Ferdinand - No You Girls',220000,'No You Girls','Franz Ferdinand','2-02 No You Girls.mp3','34.jpg','0','0','0','0',0,0,'B'),(394,'AUDIOFILE','Flo Rida Feat. Ke$ha - Right Round',207000,'Right Round','Flo Rida Feat. Ke$ha','1-18 Right Round.mp3','33.jpg','0','0','0','0',0,0,'B'),(393,'AUDIOFILE','A Fine Frenzy - Almost Lover',263000,'Almost Lover','A Fine Frenzy','Almost Lover.mp3','32.jpg','0','0','0','0',0,0,'D1'),(392,'AUDIOFILE','Emiliana Torrini - Jungle Drum',133000,'Jungle Drum','Emiliana Torrini','01 Jungle Drum.mp3','31.jpg','0','0','0','0',0,0,'A'),(391,'AUDIOFILE','Duffy - Mercy',219000,'Mercy','Duffy','Mercy.mp3','30.jpg','0','0','0','0',0,0,'D1'),(390,'AUDIOFILE','Die Ärzte - Junge',187000,'Junge','Die Ärzte','2-01 Junge.mp3','29.jpg','0','0','0','0',0,0,'B'),(389,'AUDIOFILE','Die Ärzte - Lasse redn',170000,'Lasse redn','Die Ärzte','Lasse redn.mp3','28.jpg','0','0','0','0',0,0,'D1'),(388,'AUDIOFILE','Depeche Mode - Wrong',193000,'Wrong','Depeche Mode','01 Wrong.mp3','27.jpg','0','0','0','0',0,0,'C'),(387,'AUDIOFILE','David Guetta - Kelly Rowland - When Love Takes Over (Feat.Kelly Rowland)',189000,'When Love Takes Over (Feat.Kelly Rowland)','David Guetta - Kelly Rowland','01 When Love Takes Over (Feat.Kelly.mp3','26.jpg','0','0','0','0',0,0,'A'),(386,'AUDIOFILE','David Bisbal - Silencio',211000,'Silencio','David Bisbal','Silencio.mp3','25.jpg','0','0','0','0',0,0,'D1'),(385,'AUDIOFILE','Daughtry - Over You',207000,'Over You','Daughtry','Over You.mp3','24.jpg','0','0','0','0',0,0,'D1'),(384,'AUDIOFILE','Daniel Merriweather Feat. Wale - Change',187000,'Change','Daniel Merriweather Feat. Wale','2-04 Change 1.mp3','23.jpg','0','0','0','0',0,0,'B'),(383,'AUDIOFILE','Colbie Callait - Bubbly',196000,'Bubbly','Colbie Callait','Bubbly.mp3','22.jpg','0','0','0','0',0,0,'D1'),(382,'AUDIOFILE','Colbie Caillat - Fallin\' For You',216000,'Fallin\' For You','Colbie Caillat','1-10 Fallin\' For You.mp3','21.jpg','0','0','0','0',0,0,'A'),(381,'AUDIOFILE','Christina Aguilera - Hurt',240000,'Hurt','Christina Aguilera','06 Hurt.mp3','20.jpg','0','0','0','0',0,0,'C'),(380,'AUDIOFILE','Christina Aguilera - keeps gettin better',183000,'keeps gettin better','Christina Aguilera','keeps gettin bett.mp3','19.jpg','0','0','0','0',0,0,'D1'),(379,'AUDIOFILE','Charlie Winston - Like A Hobo',218000,'Like A Hobo','Charlie Winston','01 Like A Hobo.mp3','18.jpg','0','0','0','0',0,0,'A'),(378,'AUDIOFILE','Cassandra Steen - Stadt',186000,'Stadt','Cassandra Steen','2-02 Stadt.mp3','17.jpg','0','0','0','0',0,0,'A'),(377,'AUDIOFILE','Buena Mutya - Real girl',208000,'Real girl','Buena Mutya','Real girl.mp3','16.jpg','0','0','0','0',0,0,'D2'),(376,'AUDIOFILE','The Bosshoss - I say a little prayer',172000,'I say a little prayer','The Bosshoss','I say a little prayer.mp3','15.jpg','0','0','0','0',0,0,'D2'),(375,'AUDIOFILE','Boss Hoss - Hey ya',232000,'Hey ya','Boss Hoss','Hey ya.mp3','14.jpg','0','0','0','0',0,0,'D1'),(374,'AUDIOFILE','Boney M. - Sunny (remix)',202000,'Sunny (remix)','Boney M.','Sunny (remix).mp3','13.jpg','0','0','0','0',0,0,'D2'),(373,'AUDIOFILE','Bon Jovi - Have a nice day',212000,'Have a nice day','Bon Jovi','Have a nice day.mp3','12.jpg','0','0','0','0',0,0,'D1'),(372,'AUDIOFILE','Bob Sinclar - Love generation',213000,'Love generation','Bob Sinclar','Love generation.mp3','11.jpg','0','0','0','0',0,0,'D1'),(371,'AUDIOFILE','Black Eyed Peas - Meet Me Halfway',284000,'Meet Me Halfway','Black Eyed Peas','03 Meet Me Halfway.mp3','10.jpg','0','0','0','0',0,0,'N'),(370,'AUDIOFILE','Black Eyed Peas - Boom Boom Pow',220000,'Boom Boom Pow','Black Eyed Peas','1-02 Boom Boom Pow.mp3','9.jpg','0','0','0','0',0,0,'B'),(369,'AUDIOFILE','Black Eyed Peas - I Gotta Feeling',291000,'I Gotta Feeling','Black Eyed Peas','1-01 I Gotta Feeling 1.mp3','8.jpg','0','0','0','0',0,0,'B'),(368,'AUDIOFILE','Billy Talent - Rusted From The Rain',253000,'Rusted From The Rain','Billy Talent','01 Rusted From The Rain.mp3','7.jpg','0','0','0','0',0,0,'B'),(367,'AUDIOFILE','Beyoncé - Halo',262000,'Halo','Beyoncé','2-05 Halo.mp3','6.jpg','0','0','0','0',0,0,'N'),(366,'AUDIOFILE','Backstreet Boys - Incomplete',240000,'Incomplete','Backstreet Boys','2-08 Incomplete.mp3','5.jpg','0','0','0','0',0,0,'D1'),(365,'AUDIOFILE','Avril Lavigne - Girlfriend',213000,'Girlfriend','Avril Lavigne','Girlfriend.mp3','4.jpg','0','0','0','0',0,0,'D1'),(364,'AUDIOFILE','The All-American Rejects - Gives You Hell',211000,'Gives You Hell','The All-American Rejects','1-03 Gives You Hell.mp3','3.jpg','0','0','0','0',0,0,'A'),(363,'AUDIOFILE','Agnes - Release Me',187000,'Release Me','Agnes','1-04 Release Me.mp3','2.jpg','0','0','0','0',0,0,'N'),(362,'AUDIOFILE','A-Ha - Foot Of The Mountain',224000,'Foot Of The Mountain','A-Ha','01 Foot Of The Mountain.mp3','1.jpg','0','0','0','0',0,0,'B'),(477,'AUDIOFILE','Pink - Sober',251000,'Sober','Pink','2-12 Sober.mp3','116.jpg','0','0','0','0',0,0,'C'),(478,'AUDIOFILE','Pink - so what',214000,'so what','Pink','so what.mp3','117.jpg','0','0','0','0',0,0,'D1'),(479,'AUDIOFILE','Pink - Stupid girls',193000,'Stupid girls','Pink','Stupid girls.mp3','118.jpg','0','0','0','0',0,0,'D2'),(480,'AUDIOFILE','Pink - Who knew',210000,'Who knew','Pink','Who knew.mp3','120.jpg','0','0','0','0',0,0,'D1'),(481,'AUDIOFILE','Pink - Please Don\'t Leave Me (Main Version)',231000,'Please Don\'t Leave Me (Main Version)','Pink','01 Please Don\'t Leave Me (Main Versi.mp3','121.jpg','0','0','0','0',0,0,'B'),(482,'AUDIOFILE','Pink - U + Ur Hand',211000,'U + Ur Hand','Pink','02 U + Ur Hand.mp3','122.jpg','0','0','0','0',0,0,'C'),(483,'AUDIOFILE','Pink - Nobody knows',234000,'Nobody knows','Pink','13 Nobody knowes.mp3','123.jpg','0','0','0','0',0,0,'C'),(484,'AUDIOFILE','Pitbull - I Know You Want Me (Calle Ocho)',222000,'I Know You Want Me (Calle Ocho)','Pitbull','1-06 I Know You Want Me (Calle Ocho).mp3','124.jpg','0','0','0','0',0,0,'B'),(485,'AUDIOFILE','Placebo - For What It\'s Worth',167000,'For What It\'s Worth','Placebo','1-02 For What It\'s Worth.mp3','125.jpg','0','0','0','0',0,0,'B'),(486,'AUDIOFILE','Plain White T\'s - Hey There Delilah',232000,'Hey There Delilah','Plain White T\'s','Hey There Delilah.mp3','126.jpg','0','0','0','0',0,0,'D1'),(487,'AUDIOFILE','Polarkreis 18 - allein allein',223000,'allein allein','Polarkreis 18','allein allein.mp3','127.jpg','0','0','0','0',0,0,'D1'),(488,'AUDIOFILE','The Pussycat Dolls - I Hate This Part',219000,'I Hate This Part','The Pussycat Dolls','01 I Hate This Part.mp3','128.jpg','0','0','0','0',0,0,'C'),(489,'AUDIOFILE','Queensberry - Too Young',223000,'Too Young','Queensberry','2-12 Too Young.mp3','129.jpg','0','0','0','0',0,0,'B'),(490,'AUDIOFILE','Razorlight - America',249000,'America','Razorlight','04 America.mp3','130.jpg','0','0','0','0',0,0,'C'),(491,'AUDIOFILE','Razorlight - Wire To Wire',176000,'Wire To Wire','Razorlight','1-05 Wire To Wire.mp3','131.jpg','0','0','0','0',0,0,'B'),(492,'AUDIOFILE','Reamonn - Tonight',216000,'Tonight','Reamonn','07 Tonight.mp3','132.jpg','0','0','0','0',0,0,'C'),(493,'AUDIOFILE','Red Hot Chili Peppers - Dani california',282000,'Dani california','Red Hot Chili Peppers','Dani california.mp3','133.jpg','0','0','0','0',0,0,'D1'),(494,'AUDIOFILE','Red Hot Chili Peppers - Snow',281000,'Snow','Red Hot Chili Peppers','Snow.mp3','134.jpg','0','0','0','0',0,0,'D2'),(495,'AUDIOFILE','Rihanna - Rehab',295000,'Rehab','Rihanna','2-15 Rehab.mp3','135.jpg','0','0','0','0',0,0,'C'),(496,'AUDIOFILE','Rihanna - Unfaithful',227000,'Unfaithful','Rihanna','08 Unfaithful.mp3','136.jpg','0','0','0','0',0,0,'C'),(497,'AUDIOFILE','Rihanna feat. Ne-Yo - Hate That I Love You',219000,'Hate That I Love You','Rihanna feat. Ne-Yo','Hate That I Love You.mp3','137.jpg','0','0','0','0',0,0,'D1'),(498,'AUDIOFILE','Rob Thomas - Little wonders',229000,'Little wonders','Rob Thomas','Little wonders.mp3','138.jpg','0','0','0','0',0,0,'D1'),(499,'AUDIOFILE','Rob Thomas - Won\'t go home without you',231000,'Won\'t go home without you','Rob Thomas','Won\'t go home without you.mp3','139.jpg','0','0','0','0',0,0,'D2'),(500,'AUDIOFILE','Robbie Williams - Rudebox',226000,'Rudebox','Robbie Williams','Rudebox.mp3','140.jpg','0','0','0','0',0,0,'D2'),(501,'AUDIOFILE','Robbie Williams - Tripping',224000,'Tripping','Robbie Williams','Tripping.mp3','141.jpg','0','0','0','0',0,0,'D1'),(502,'AUDIOFILE','Robbie Williams & Pet Shop Boys; Robbie Williams - She´s Madonna',239000,'She´s Madonna','Robbie Williams & Pet Shop Boys; Robbie Williams','01 She_s Madonna.mp3','142.jpg','0','0','0','0',0,0,'C'),(503,'AUDIOFILE','Roger Cicero - Fraun regiern die welt',179000,'Fraun regiern die welt','Roger Cicero','Fraun regiern die welt.mp3','143.jpg','0','0','0','0',0,0,'D2'),(504,'AUDIOFILE','Roger Cicero - Zieh die schuh aus',197000,'Zieh die schuh aus','Roger Cicero','Zieh die schuh aus.mp3','144.jpg','0','0','0','0',0,0,'D2'),(505,'AUDIOFILE','Rooney - when did your heart go missin',209000,'when did your heart go missin','Rooney','when did your heart go missin.mp3','145.jpg','0','0','0','0',0,0,'D1'),(506,'AUDIOFILE','Rosenstolz - Ich Bin Ich (Wir Sind Wir)',216000,'Ich Bin Ich (Wir Sind Wir)','Rosenstolz','06 Ich Bin Ich (Wir Sind Wir).mp3','146.jpg','0','0','0','0',0,0,'C'),(507,'AUDIOFILE','Rosenstolz - Ich geh in Flammen auf',227000,'Ich geh in Flammen auf','Rosenstolz','11 Ich geh in Flammen auf.mp3','147.jpg','0','0','0','0',0,0,'C'),(508,'AUDIOFILE','Sarah Connor - Son of a preacher man',152000,'Son of a preacher man','Sarah Connor','Son of a preacher man.mp3','148.jpg','0','0','0','0',0,0,'D2'),(509,'AUDIOFILE','Sarah Connor - From Zero To Hero',228000,'From Zero To Hero','Sarah Connor','2-21 From Zero To Hero.mp3','149.jpg','0','0','0','0',0,0,'D1'),(510,'AUDIOFILE','Sasha - Lucky day',197000,'Lucky day','Sasha','Lucky day.mp3','150.jpg','0','0','0','0',0,0,'D1'),(511,'AUDIOFILE','Sasha - Slowly',246000,'Slowly','Sasha','07 Slowly.mp3','151.jpg','0','0','0','0',0,0,'C'),(512,'AUDIOFILE','Scissor Sisters - I Don\'t Feel Like Dancin\'',248000,'I Don\'t Feel Like Dancin\'','Scissor Sisters','01 I Don\'t Feel Like Dancin\'.mp3','153.jpg','0','0','0','0',0,0,'C'),(513,'AUDIOFILE','Seal - Amazing',188000,'Amazing','Seal','Amazing.mp3','154.jpg','0','0','0','0',0,0,'D1'),(514,'AUDIOFILE','Sebastian Hämer - Sommer unseres lebens',243000,'Sommer unseres lebens','Sebastian Hämer','Sommer unseres lebens.mp3','155.jpg','0','0','0','0',0,0,'D2'),(515,'AUDIOFILE','Sercio Mendes& Black Eyed Peas - Mas que nada',230000,'Mas que nada','Sercio Mendes& Black Eyed Peas','Mas que nada.mp3','156.jpg','0','0','0','0',0,0,'D1'),(516,'AUDIOFILE','Shaggy - feel the rush',186000,'feel the rush','Shaggy','feel the rush.mp3','157.jpg','0','0','0','0',0,0,'D2'),(517,'AUDIOFILE','Shakira - She Wolf',187000,'She Wolf','Shakira','1-02 She Wolf.mp3','158.jpg','0','0','0','0',0,0,'N'),(518,'AUDIOFILE','Shakira Feat. Carlos Santana - Illegal',234000,'Illegal','Shakira Feat. Carlos Santana','03 Illegal.mp3','160.jpg','0','0','0','0',0,0,'C'),(519,'AUDIOFILE','Shakira Feat. Wyclef Jean - Hips Don´t Lie',218000,'Hips Don´t Lie','Shakira Feat. Wyclef Jean','05 Hips Don_t Lie.mp3','161.jpg','0','0','0','0',0,0,'C'),(520,'AUDIOFILE','Shinedown - Second Chance (Album Version)',220000,'Second Chance (Album Version)','Shinedown','01 Second Chance (Album Version).mp3','162.jpg','0','0','0','0',0,0,'B'),(521,'AUDIOFILE','Silbermond - Irgendwas Bleibt',194000,'Irgendwas Bleibt','Silbermond','2-09 Irgendwas Bleibt.mp3','163.jpg','0','0','0','0',0,0,'B'),(522,'AUDIOFILE','Silbermond - Das Beste',264000,'Das Beste','Silbermond','05 Das Beste.mp3','165.jpg','0','0','0','0',0,0,'C'),(523,'AUDIOFILE','Snow Patrol - Chasing Cars',265000,'Chasing Cars','Snow Patrol','13 Chasing Cars.mp3','166.jpg','0','0','0','0',0,0,'C'),(524,'AUDIOFILE','Snow Patrol - Crack The Shutters',200000,'Crack The Shutters','Snow Patrol','1-14 Crack The Shutters.mp3','167.jpg','0','0','0','0',0,0,'B'),(525,'AUDIOFILE','Sportfreunde Stiller - 54749020010',187000,'54749020010','Sportfreunde Stiller','54749020010.mp3','169.jpg','0','0','0','0',0,0,'D2'),(526,'AUDIOFILE','Sportfreunde Stiller - Ein Kompliment (unplugged)',266000,'Ein Kompliment (unplugged)','Sportfreunde Stiller','01 Ein Kompliment.mp3','170.jpg','0','0','0','0',0,0,'B'),(527,'AUDIOFILE','Stacie Orrico - I´m Not Missing You',221000,'I´m Not Missing You','Stacie Orrico','19 I_m Not Missing You.mp3','171.jpg','0','0','0','0',0,0,'C'),(528,'AUDIOFILE','Stanfour - For All Lovers',230000,'For All Lovers','Stanfour','For All Lovers.mp3','172.jpg','0','0','0','0',0,0,'D2'),(529,'AUDIOFILE','Stefanie Heinzmann - No One (Can Ever Change My Mind)',215000,'No One (Can Ever Change My Mind)','Stefanie Heinzmann','1-13 No One (Can Ever Change My Mind.mp3','173.jpg','0','0','0','0',0,0,'B'),(530,'AUDIOFILE','Stefanie Heinzmann -  My Man Is A Mean Man',211000,' My Man Is A Mean Man','Stefanie Heinzmann','_My Man Is A Mean Man.mp3','174.jpg','0','0','0','0',0,0,'D1'),(531,'AUDIOFILE','Sugababes - About you now',212000,'About you now','Sugababes','About you now.mp3','175.jpg','0','0','0','0',0,0,'D1'),(532,'AUDIOFILE','Sugababes - Push The Button',215000,'Push The Button','Sugababes','1-02 Push The Button.mp3','176.jpg','0','0','0','0',0,0,'D1'),(533,'AUDIOFILE','Sunrise Avenue - Fairytale gone bad',213000,'Fairytale gone bad','Sunrise Avenue','Fairytale gone bad.mp3','177.jpg','0','0','0','0',0,0,'D1'),(534,'AUDIOFILE','Sunrise Avenue - The Whole Story',214000,'The Whole Story','Sunrise Avenue','01 The Whole Story.mp3','178.jpg','0','0','0','0',0,0,'B'),(535,'AUDIOFILE','Texas Lightning - No no never',179000,'No no never','Texas Lightning','No no never.mp3','179.jpg','0','0','0','0',0,0,'D1'),(536,'AUDIOFILE','Timbaland - Apologize',184000,'Apologize','Timbaland','Apologize.mp3','180.jpg','0','0','0','0',0,0,'D1'),(537,'AUDIOFILE','The Ting Tings - Shut Up And Let Me Go',172000,'Shut Up And Let Me Go','The Ting Tings','02 Shut Up And Let Me Go.mp3','181.jpg','0','0','0','0',0,0,'C'),(538,'AUDIOFILE','Ville Valo & Natalia Avelon - Summer Wine',233000,'Summer Wine','Ville Valo & Natalia Avelon','19 Summer Wine.mp3','182.jpg','0','0','0','0',0,0,'C'),(539,'AUDIOFILE','Wir sind Helden - Nur ein Wort',235000,'Nur ein Wort','Wir sind Helden','Nur ein Wort.mp3','183.jpg','0','0','0','0',0,0,'D2'),(540,'AUDIOFILE','Xavier Naidoo - Dieser Weg',244000,'Dieser Weg','Xavier Naidoo','2-05 Dieser Weg.mp3','184.jpg','0','0','0','0',0,0,'C'),(541,'AUDIOFILE','Yael Naim - New Soul',226000,'New Soul','Yael Naim','New Soul.mp3','185.jpg','0','0','0','0',0,0,'D1'),(542,'AUDIOFILE','Jingle - Slow',6000,'Slow','Jingle','zjingle-slow.mp3','0','0','0','0','0',0,0,'J'),(543,'AUDIOFILE','Jingle - Medium',4000,'Medium','Jingle','zjingle-medium.mp3','0','0','0','0','0',0,0,'J'),(544,'AUDIOFILE','Jingle - Slow-Fast',8000,'Slow-Fast','Jingle','zjingle-slow-fast.mp3','0','0','0','0','0',0,0,'J'),(545,'AUDIOFILE','Jingle - Fast-Slow',3000,'Fast-Slow','Jingle','zjingle-fast-slow.mp3','0','0','0','0','0',0,0,'J'),(546,'AUDIOFILE','Jingle - Fast',10000,'Fast','Jingle','zjingle-fast.mp3','0','0','0','0','0',0,0,'J'),(547,'AUDIOFILE','Promo Moshow - Moshow 1',0,'Moshow 1','Promo Moshow','','0','0','0','0','0',0,0,'P'),(548,'AUDIOFILE','Promo Major Promotion - Major Promotion 1',0,'Major Promotion 1','Promo Major Promotion','','0','0','0','0','0',0,0,'P'),(549,'AUDIOFILE','Promo Event - Event 01',0,'Event 01','Promo Event','','0','0','0','0','0',0,0,'P'),(550,'AUDIOFILE','Promo Moshow - Moshow 2',0,'Moshow 2','Promo Moshow','','0','0','0','0','0',0,0,'P'),(551,'AUDIOFILE','Promo Major Promotion - Major Promotion 2',0,'Major Promotion 2','Promo Major Promotion','','0','0','0','0','0',0,0,'P'),(552,'AUDIOFILE','Promo Major Promotion - Major Promotion 3',0,'Major Promotion 3','Promo Major Promotion','','0','0','0','0','0',0,0,'P'),(553,'AUDIOFILE','Promo Major Promotion - Major Promotion 4',0,'Major Promotion 4','Promo Major Promotion','','0','0','0','0','0',0,0,'P'),(554,'AUDIOFILE','Promo Major Promotion - Major Promotion 5',0,'Major Promotion 5','Promo Major Promotion','','0','0','0','0','0',0,0,'P'),(555,'AUDIOFILE','Promo Event - Event 02',0,'Event 02','Promo Event','','0','0','0','0','0',0,0,'P'),(556,'AUDIOFILE','Promo Event - Event 03',0,'Event 03','Promo Event','','0','0','0','0','0',0,0,'P'),(557,'AUDIOFILE','Promo Event - Event 04',0,'Event 04','Promo Event','','0','0','0','0','0',0,0,'P'),(558,'AUDIOFILE','Promo Event - Event 05',0,'Event 05','Promo Event','','0','0','0','0','0',0,0,'P'),(559,'AUDIOFILE','Promo Event - Event 06',0,'Event 06','Promo Event','','0','0','0','0','0',0,0,'P'),(560,'AUDIOFILE','Promo Event - Event 07',0,'Event 07','Promo Event','','0','0','0','0','0',0,0,'P'),(561,'AUDIOFILE','Promo Event - Event 08',0,'Event 08','Promo Event','','0','0','0','0','0',0,0,'P'),(562,'AUDIOFILE','Promo Event - Event 09',0,'Event 09','Promo Event','','0','0','0','0','0',0,0,'P'),(563,'AUDIOFILE','Promo Event - Event 10',0,'Event 10','Promo Event','','0','0','0','0','0',0,0,'P'),(564,'AUDIOFILE','Promo Drive Time - Promo Drive Time 1',0,'Promo Drive Time 1','Promo Drive Time','','0','0','0','0','0',0,0,'P'),(565,'AUDIOFILE','Promo Drive Time - Promo Drive Time 2',0,'Promo Drive Time 2','Promo Drive Time','','0','0','0','0','0',0,0,'P'),(566,'AUDIOFILE','Promo Moshow - Moshow 3',0,'Moshow 3','Promo Moshow','','0','0','0','0','0',0,0,'P'),(567,'AUDIOFILE','Promo Drive Time - Promo Drive Time 3',0,'Promo Drive Time 3','Promo Drive Time','','0','0','0','0','0',0,0,'P'),(568,'AUDIOFILE','Promo Drive Time - Promo Drive Time 4',0,'Promo Drive Time 4','Promo Drive Time','','0','0','0','0','0',0,0,'P'),(569,'AUDIOFILE','Promo Drive Time - Promo Drive Time 5',0,'Promo Drive Time 5','Promo Drive Time','','0','0','0','0','0',0,0,'P'),(570,'AUDIOFILE','Promo Moshow - Moshow 4',0,'Moshow 4','Promo Moshow','','0','0','0','0','0',0,0,'P'),(571,'AUDIOFILE','Promo Moshow - Moshow 5',0,'Moshow 5','Promo Moshow','','0','0','0','0','0',0,0,'P'),(572,'AUDIOFILE','Promo Image - Image 01',0,'Image 01','Promo Image','','0','0','0','0','0',0,0,'P'),(573,'AUDIOFILE','Promo Image - Image 02',0,'Image 02','Promo Image','','0','0','0','0','0',0,0,'P'),(574,'AUDIOFILE','Promo Image - Image 03',0,'Image 03','Promo Image','','0','0','0','0','0',0,0,'P'),(575,'AUDIOFILE',' - CTBTO',337000,'CTBTO','','news-ctbto.mp3','4926596814_a277e7b5e3_o.jpg','4926598296_19892a98ea_o.jpg','4928739825_9feeff670c_o.jpg','4926598296_19892a98ea_o.jpg','The International Day Against Nuclear Tests is marked on 29 August 2010, organised by the Preparatory Commission for the Comprehensive Nuclear-Test-Ban Treaty Organization (CTBTO)',0,0,'NWS'),(576,'AUDIOFILE',' - G8-Harper',109000,'G8-Harper','','G8-Harper.mp3','1-g8.jpeg','1-g8.jpeg','1-g8.jpeg','1-g8.jpeg','Current G8 and Canadian President Stephen Harper addresses the G8 summit held in Muskoka, Canada on 25-26 June 2010',0,0,'NWS'),(577,'AUDIOFILE',' - IAAF',105000,'IAAF','','IAAF.mp3','2.jpg','3.jpg','4.jpg','5.jpg','The 2010 IAAF European Athletics Championships were held in Barcelona from 26 July - 1 August 2010',0,0,'NWS'),(578,'AUDIOFILE',' - MSF-Pakistan',60000,'MSF-Pakistan','','MSF-Pakistan.mp3','msf1.jpeg','msf2.jpeg','msf3.jpeg','msf4.jpeg','Médecins Sans Frontières (MSF) continues to scale up its activities in the flood-affected areas of Pakistan',0,0,'NWS'),(579,'AUDIOFILE',' - WEF Davos',66000,'WEF Davos','','WEF-Davos.mp3','am10_aaam8186.jpg','am10_aaam8204.jpg','am10_rst03653.jpg','am10_rst352140.jpg','Former US President Bill Clinton appeals for aid at a special session on Haiti at the World Economic Forum in Davos, Switzerland in January 2010.',0,0,'NWS'),(580,'AUDIOFILE','Top Horaire - IBC 2010',7000,'IBC 2010','Top Horaire','toph-ibc.mp3','0','0','0','0','0',0,0,'T'),(581,'AUDIOFILE','News - IN',6000,'IN','News','zjingle-news-in.mp3','0','0','0','0','0',0,0,'JN'),(582,'AUDIOFILE','News - OUT',2000,'OUT','News','zjingle-news-out.mp3','0','0','0','0','0',0,0,'JN');
/*!40000 ALTER TABLE `dataitems` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2011-01-20  2:38:20
