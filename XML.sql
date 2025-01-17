create database ValidadorXml;
use ValidadorXml;

create table Estatus(
ESTATUS_ID int identity (1,1) primary key,
DESCRIPCION nvarchar (50)
);

create table Informacion(
ID int identity (1,1) primary key,
RFC_EMISOR nvarchar (13),
RFC_RECEPTOR nvarchar (13),
FOLIO_FISCAL char (36),
FECHA_EMISION datetime,
TOTAL decimal (18,2),
ESTATUS_ID int,
constraint FK_CFDI_ESTATUS foreign key (ESTATUS_ID) references Estatus(ESTATUS_ID)
);

