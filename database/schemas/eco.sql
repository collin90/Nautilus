CREATE SCHEMA eco;
GO

CREATE TABLE eco.kingdom (
    id      INT PRIMARY KEY,
    name    VARCHAR(256) NOT NULL UNIQUE
);

CREATE TABLE eco.phylum (
    id          INT PRIMARY KEY,
    name        VARCHAR(256) NOT NULL,
    kingdom_id  INT NOT NULL REFERENCES eco.kingdom(id),
    
    UNIQUE(name, kingdom_id)
);

CREATE TABLE eco.class (
    id         INT PRIMARY KEY,
    name       VARCHAR(256) NOT NULL,
    phylum_id  INT NOT NULL REFERENCES eco.phylum(id),

    UNIQUE(name, phylum_id)
);

CREATE TABLE eco.[order] (
    id        INT PRIMARY KEY,
    name      VARCHAR(256) NOT NULL,
    class_id  INT NOT NULL REFERENCES eco.class(id),

    UNIQUE(name, class_id)
);

CREATE TABLE eco.family (
    id        INT PRIMARY KEY,
    name      VARCHAR(256) NOT NULL,
    order_id  INT NOT NULL REFERENCES eco.[order](id),

    UNIQUE(name, order_id)
);

CREATE TABLE eco.genus (
    id         INT PRIMARY KEY,
    name       VARCHAR(256) NOT NULL,
    family_id  INT NOT NULL REFERENCES eco.family(id),

    UNIQUE(name, family_id)
);

CREATE TABLE eco.species (
    id              INT PRIMARY KEY,
    scientfic_name  VARCHAR(256) NOT NULL,
    genus_id        INT NOT NULL REFERENCES eco.genus(id),
    usage_key       BIGINT UNIQUE,
    authorship      TEXT,
    gbif_link       TEXT
);

CREATE TABLE eco.taxon_common_name (
    id             UNIQUEIDENTIFIER PRIMARY KEY,
    species_id     INT REFERENCES eco.species(id) ON DELETE CASCADE,
    common_name    TEXT NOT NULL,
    language       TEXT,
    source         TEXT,
    is_preferred   BIT DEFAULT 0
);

CREATE TABLE eco.search_cache (
    query_text    NVARCHAR(256) PRIMARY KEY,
    species_id    INT REFERENCES eco.species(id),
    confidence    NUMERIC(4,3),
    created_at    DATETIME DEFAULT GetUTCDate()
);

