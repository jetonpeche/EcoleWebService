CREATE TABLE TypeRestaurant(
    Id int PRIMARY KEY AUTO_INCREMENT,

    Nom varchar(200) NOT NULL
);

CREATE TABLE Personne(
    Id int PRIMARY KEY AUTO_INCREMENT,

    Nom varchar(200) NOT NULL,
    Login varchar(200) NOT NULL,
    Mdp varchar(400) NOT NULL,
    EstAdmin TINYINT(1) DEFAULT 0
);

CREATE TABLE Restaurant(
    Id int PRIMARY KEY AUTO_INCREMENT,
    IdTypeRestaurant int NOT NULL,

    Nom varchar(200) NOT NULL,
    Adresse varchar(300) NOT NULL,
    Description varchar(1000) NULL,
    Url varchar(1000) NULL,
    Telephone char(10) NULL,

    FOREIGN KEY (IdTypeRestaurant) REFERENCES TypeRestaurant(Id)
);

CREATE TABLE PersonneRestaurantAimer(
    IdPersonne int NOT NULL,
    IdRestaurant int NOT NULL,

    PRIMARY KEY (IdRestaurant, IdPersonne),

    FOREIGN KEY (IdPersonne) REFERENCES Personne(Id) ON DELETE CASCADE,
    FOREIGN KEY (IdRestaurant) REFERENCES Restaurant(Id) ON DELETE CASCADE
);

CREATE TABLE PersonneRestaurantHistorique(

    Id int PRIMARY KEY AUTO_INCREMENT,

    IdPersonne int NOT NULL,
    IdRestaurant int NOT NULL,

    Date DATE NOT NULL,
    Prix decimal(5, 2) DEFAULT 0 NOT NULL,

    FOREIGN KEY (IdPersonne) REFERENCES Personne(Id) ON DELETE CASCADE,
    FOREIGN KEY (IdRestaurant) REFERENCES Restaurant(Id) ON DELETE CASCADE
);

INSERT INTO Personne (Id, Nom, Login, Mdp) VALUES (1, "Nicolas", "Login1", "qZ0k6hASxdburWim8ibpZg==$2lFqnNb5QrqJwalpUTYMgZWvEzz+csPKyMeHdxWs0vM=");

INSERT INTO TypeRestaurant (Id, Nom) VALUES
(1, "Boulangerie"), (2, "Stack"), (3, "Pizzeria"), 
(4, "Kebab"), (5, "japonnais"), (6, "Chinois"),
(7, "Restaurant");

INSERT INTO Restaurant (Id, IdTypeRestaurant, Nom, Adresse, Description, Url, Telephone) VALUES
(1, 1, "Boulangerie Prestipino Baguettes Sandwichs & Tradition", "13 Av. Antoine Dutrievoz, 69100 Villeurbanne", "A coté de l'arret de bus", null, null),
(2, 1, "La Tradition", "102 Cr Vitton, 69006 Lyon", "Devanture rouge", null, null),
(3, 7, "Bill's Burger", "89 Cr Vitton, 69006 Lyon", null, "https://bills-burger.fr/menu/", null),
(4, 7, "Sucré & Salé", "87 Cr Vitton, 69006 Lyon", "Vente de plats chauds, paninis", null, null),
(5, 1, "Boulangerie Ararat", "37 Cr André Philip, 69100 Villeurbanne", "paninis, mini pizza, sandwitch", null, null),
(6, 5, "D.D Sushi", "128 Av. Thiers, 69006 Lyon", null, "http://ddsushi.fr/index.php?route=product/category&path=67", "0673339299"),
(7, 5, "WAKAI", "10 Av. Antoine Dutrievoz, 69100 Villeurbanne", null, "http://www.wakaisushi.fr/menu/", "0478948698");