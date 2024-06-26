--The data was generated by AI and may be fake

DECLARE @ImageUri VARCHAR(100) = 'https://hoppyhub.blob.core.windows.net/hoppyhub/Beers/temp.jpg'

-- IPA style ids
DECLARE @WestCoastIPAStyleId NVARCHAR(36) = '0f9a4cc3-25a9-41e7-8b50-7b1542824b2c';
DECLARE @NewEnglandIPAStyleId NVARCHAR(36) = 'a4cf7d47-b66e-46f3-a1f1-df8b12293031';
DECLARE @DoubleIPAStyleId NVARCHAR(36) = 'e4b5f90e-6c2a-42ec-99c8-c3c1a8e9fdda';
DECLARE @BlackIPAStyleId NVARCHAR(36) = 'e0dbb10c-9c6b-408c-91e8-35a74d9b0ef4';
DECLARE @BelgianIPAStyleId NVARCHAR(36) = 'c8eac9bb-45df-43c6-b02d-8ca5ec2bb320';

-- Stout style ids
DECLARE @DryStoutStyleId NVARCHAR(36) = 'd1afbd9f-3c4e-4df2-bfc7-7c1e97b3b4af';
DECLARE @SweetStoutStyleId NVARCHAR(36) = '4634297d-cdd3-4a0d-8ec5-0a4e4c34a84d';
DECLARE @ImperialStoutStyleId NVARCHAR(36) = 'ae372e88-5e43-49b4-9d7d-0c2746cdd401';
DECLARE @OatmealStoutStyleId NVARCHAR(36) = '7dd1d5c9-fc70-4b2d-a78a-019d65743d6a';
DECLARE @ForeignExtraStoutStyleId NVARCHAR(36) = '0574e07c-05b7-4c68-a7c3-ef7e871a75a1';

-- Pilsner style ids
DECLARE @GermanPilsnerStyleId NVARCHAR(36) = '4fa0b62a-b15d-4752-b7f2-320c31c3c172';
DECLARE @BohemianPilsnerStyleId NVARCHAR(36) = '83f6a918-1462-44e6-9466-8d8f03e33e3a';
DECLARE @AmericanPilsnerStyleId NVARCHAR(36) = '7c4bdc04-6b4c-4012-ae16-d47ba1d5d7c7';
DECLARE @ItalianPilsnerStyleId NVARCHAR(36) = 'bc7b6c72-4f4f-45dd-a7a7-2b1c4cd5f80f';
DECLARE @MexicanPilsnerStyleId NVARCHAR(36) = 'db67cf9b-4316-4fa6-8d84-d1a4d977d4f9';

-- Witbier style ids
DECLARE @BelgianWitbierStyleId NVARCHAR(36) = '00f6aa3d-4d04-438d-a77a-7d198f3f3b6f';
DECLARE @AmericanWheatStyleId NVARCHAR(36) = 'c9f40191-5241-4c31-80f5-40845d1e7e8d';
DECLARE @GoseStyleId NVARCHAR(36) = '16d799cf-05e9-4069-b7e1-3b1415f8c2b9';
DECLARE @WhiteIPAStyleId NVARCHAR(36) = '201cc54b-5e99-4756-af29-4b2c2335a9f3';

-- APA style ids
DECLARE @AmericanPaleAleStyleId NVARCHAR(36) = 'f2e167c5-06b6-41f4-b4d1-7fdeaa44b30b';
DECLARE @ExtraPaleAleStyleId NVARCHAR(36) = '5f7c9e3d-50b8-4c94-9e61-d6af8b09fae9';
DECLARE @BelgianPaleAleStyleId NVARCHAR(36) = 'a0ef1dfc-2dbb-43d3-a2e8-28b46ecaa98c';
DECLARE @EnglishPaleAleStyleId NVARCHAR(36) = 'bcb6a232-b6f4-497d-b84a-9d023dd3d063';
DECLARE @SessionPaleAleStyleId NVARCHAR(36) = '68e3b11c-09c2-4842-81e6-c7dbcc6a3aa5';

-- Porter style ids
DECLARE @EnglishPorterStyleId NVARCHAR(36) = '5b5a29c8-98f6-482c-bc50-5c1f4a8005e1';
DECLARE @AmericanPorterStyleId NVARCHAR(36) = '17c8718d-9ec3-4c5f-9c24-5c025a013a10';
DECLARE @BalticPorterStyleId NVARCHAR(36) = 'c8da23a3-9d95-48d5-bfb6-5f7e066dd17a';
DECLARE @RobustPorterStyleId NVARCHAR(36) = 'f7fa33f2-7f24-45c2-8d53-523f6d6a35cb';
DECLARE @SmokedPorterStyleId NVARCHAR(36) = '482b031d-9c33-4478-bbba-45a026cf8c1d';

-- Hefeweizen style ids
DECLARE @GermanHefeweizenStyleId NVARCHAR(36) = '2d1dc2a9-44dd-40c3-afbb-6d8712aa5085';
DECLARE @AmericanHefeweizenStyleId NVARCHAR(36) = '7e768d2a-7af3-432a-8587-5870c8df032c';
DECLARE @DunkelweizenStyleId NVARCHAR(36) = 'ebc02b64-40d5-4a7a-97eb-090bbdd726aa';
DECLARE @WeizenbockStyleId NVARCHAR(36) = 'c4b4e6ed-2f19-4480-913e-1eb0d8363f46';
DECLARE @KristallweizenStyleId NVARCHAR(36) = 'a2f5c0a6-53c7-4a52-a5d8-73a51a9e6cc4';

-- Sour Ale style ids
DECLARE @FlandersRedAleStyleId NVARCHAR(36) = 'c69b5b71-5d05-4c5e-a79e-f49c3ab87c62';
DECLARE @GueuzeStyleId NVARCHAR(36) = '13f46e45-cacb-4d4a-9e48-c1d8c3ad8202';
DECLARE @BerlinerWeisseStyleId NVARCHAR(36) = '0cbb0e5a-58d1-4ee1-a3e5-0674aaf88975';
DECLARE @AmericanWildAleStyleId NVARCHAR(36) = '902f4082-4223-44b1-9e37-bf7d3d9d29e4';

-- Brown Ale style ids
DECLARE @EnglishBrownAleStyleId NVARCHAR(36) = 'b30034aa-0d0d-4e6e-b6f6-18db7944d3f6';
DECLARE @AmericanBrownAleStyleId NVARCHAR(36) = '6b4e0e9f-ee9c-4d09-bd3b-58489d1425ea';
DECLARE @MildAleStyleId NVARCHAR(36) = '4619a1ec-290f-4872-a087-5be8c133eb43';
DECLARE @NorthernEnglishBrownAleStyleId NVARCHAR(36) = 'b82eab99-1f08-46d7-bbfd-dc2ca8d868d3';

-- Dubbel style ids
DECLARE @BelgianDubbelStyleId NVARCHAR(36) = '0f7e3a3d-6b16-4d4f-8378-6d19f981c7df';
DECLARE @AbbeyDubbelStyleId NVARCHAR(36) = '6935a2bf-c9bb-4bcf-9b37-1cd89e6f9825';
DECLARE @DubbelIPAStyleId NVARCHAR(36) = 'bc5b4552-daa1-4e9f-af5c-e1007e9df647';
DECLARE @SmokedDubbelStyleId NVARCHAR(36) = 'f0e5a11c-d6c7-43d9-a0ab-1d27d8c9d4a5';
DECLARE @FruitDubbelStyleId NVARCHAR(36) = '19edde2a-af0d-46c9-8c6f-0a091e65b5da';

-- Stu Mostów brewery
DECLARE @BrowarStuMostówId NVARCHAR(36) = '9f7fbbcf-75c9-4457-bfef-bf940eb51ccf';

INSERT INTO Beers (Id, Name, AlcoholByVolume, Blg, Ibu, Description, BreweryId, BeerStyleId, Composition, ReleaseDate)
VALUES (NEWID(), 'West Coast Punch', 7.2, 16, 70, 'Intensely hopped with citrus and pine notes',
        @BrowarStuMostówId, @WestCoastIPAStyleId,
        'Pale ale malt, caramel malt, Cascade hops, Simcoe hops, Chinook hops',
        DATEADD(day, ABS(CHECKSUM(NEWID())) % 8646, '2000-01-01')),
       (NEWID(), 'Hazy Memory', 6.8, 16, 45, 'Juicy with tropical fruit flavors and low bitterness',
        @BrowarStuMostówId, @NewEnglandIPAStyleId,
        'Pale ale malt, wheat malt, oats, Citra hops, Mosaic hops, Galaxy hops',
        DATEADD(day, ABS(CHECKSUM(NEWID())) % 8646, '2000-01-01')),
       (NEWID(), 'Double Dry Hop Mosaic', 8.5, 18, 80, 'Big and bold with resinous pine and grapefruit flavors',
        @BrowarStuMostówId, @DoubleIPAStyleId, 'Pale ale malt, crystal malt, Munich malt, Mosaic hops',
        DATEADD(day, ABS(CHECKSUM(NEWID())) % 8646, '2000-01-01')),
       (NEWID(), 'Belgian Black Diamond', 7.5, 17, 50, 'Spicy and fruity with a dry finish', @BrowarStuMostówId,
        @BelgianIPAStyleId, 'Pilsner malt, wheat malt, caramel malt, Belgian yeast, Saaz hops, Styrian Goldings hops',
        DATEADD(day, ABS(CHECKSUM(NEWID())) % 8646, '2000-01-01')),
       (NEWID(), 'Cocoa Muffin', 6.0, 14, 30, 'Smooth and creamy with chocolate and coffee flavors',
        @BrowarStuMostówId, @SweetStoutStyleId,
        'Pale ale malt, crystal malt, chocolate malt, oats, lactose, East Kent Goldings hops',
        DATEADD(day, ABS(CHECKSUM(NEWID())) % 8646, '2000-01-01'));

-- Pinta brewery
DECLARE @BrowarPintaId NVARCHAR(36) = 'e5631c02-d66a-468c-992f-1bffe9a187ec';

INSERT INTO Beers (Id, Name, AlcoholByVolume, Blg, Ibu, Description, BreweryId, BeerStyleId, Composition, ReleaseDate)
VALUES (NEWID(), 'West Coast Warrior', 6.5, 16, 60, 'Classic West Coast IPA', @BrowarPintaId, @WestCoastIPAStyleId,
        'Water, malted barley, hops, yeast', DATEADD(day, ABS(CHECKSUM(NEWID())) % 8646, '2000-01-01')),
       (NEWID(), 'New England Haze', 6.2, 14, 40, 'Hazy and juicy IPA', @BrowarPintaId, @NewEnglandIPAStyleId,
        'Water, malted barley, wheat, oats, hops, yeast', DATEADD(day, ABS(CHECKSUM(NEWID())) % 8646, '2000-01-01')),
       (NEWID(), 'Double Dragon', 8.0, 18, 90, 'Double IPA with citrus and pine notes', @BrowarPintaId,
        @DoubleIPAStyleId, 'Water, malted barley, hops, yeast',
        DATEADD(day, ABS(CHECKSUM(NEWID())) % 8646, '2000-01-01')),
       (NEWID(), 'Black Mamba', 7.0, 18, 70, 'Black IPA with roasted malt and tropical fruit notes',
        @BrowarPintaId, @BlackIPAStyleId, 'Water, malted barley, roasted barley, hops, yeast',
        DATEADD(day, ABS(CHECKSUM(NEWID())) % 8646, '2000-01-01')),
       (NEWID(), 'Belgian White Knight', 5.5, 12, 20, 'Belgian-style IPA with coriander and orange peel',
        @BrowarPintaId, @BelgianIPAStyleId, 'Water, malted barley, wheat, hops, yeast, coriander, orange peel',
        DATEADD(day, ABS(CHECKSUM(NEWID())) % 8646, '2000-01-01'));

-- Artezan brewery
DECLARE @BrowarArtezanId NVARCHAR(36) = 'be8578e1-d5bb-43ca-8ab2-0c625314c078';

INSERT INTO Beers (Id, Name, AlcoholByVolume, Blg, Ibu, Description, BreweryId, BeerStyleId, Composition, ReleaseDate)
VALUES (NEWID(), 'Artezan Summer Breeze', 4.7, 11.5, 25,
        'Refreshing American Pilsner with citrusy hops aroma and light bitterness', @BrowarArtezanId,
        @AmericanPilsnerStyleId, 'water, barley malt, wheat malt, hops, yeast',
        DATEADD(day, ABS(CHECKSUM(NEWID())) % 8646, '2000-01-01')),
       (NEWID(), 'Artezan Nutty Brown', 5.0, 13.0, 25,
        'English Brown Ale with toasty, nutty malt flavor and light hop bitterness', @BrowarArtezanId,
        @EnglishBrownAleStyleId, 'water, barley malt, crystal malt, chocolate malt, hops, yeast',
        DATEADD(day, ABS(CHECKSUM(NEWID())) % 8646, '2000-01-01')),
       (NEWID(), 'Artezan Oatmeal Joy', 6.0, 16.5, 40,
        'Smooth and creamy Oatmeal Stout with coffee and chocolate notes', @BrowarArtezanId, @OatmealStoutStyleId,
        'water, barley malt, oatmeal, hops, yeast', DATEADD(day, ABS(CHECKSUM(NEWID())) % 8646, '2000-01-01')),
       (NEWID(), 'Artezan Bohemian Rhapsody', 5.5, 12.5, 35,
        'Bohemian Pilsner with spicy Saaz hops and bready malt character', @BrowarArtezanId, @BohemianPilsnerStyleId,
        'water, barley malt, Saaz hops, yeast', DATEADD(day, ABS(CHECKSUM(NEWID())) % 8646, '2000-01-01')),
       (NEWID(), 'Artezan Baltic Nights', 9.0, 25.0, 60,
        'Strong and rich Baltic Porter with dark fruit and roasted malt flavors', @BrowarArtezanId,
        @BalticPorterStyleId, 'water, barley malt, caramel malt, chocolate malt, hops, yeast',
        DATEADD(day, ABS(CHECKSUM(NEWID())) % 8646, '2000-01-01'));

-- Trzech Kumpli brewery
DECLARE @BrowarTrzechKumpliId NVARCHAR(36) = 'fdbc7c28-34f9-4963-a17e-176f176bab89';

INSERT INTO Beers (Id, Name, AlcoholByVolume, Blg, Ibu, Description, BreweryId, BeerStyleId, Composition, ReleaseDate)
VALUES (NEWID(), 'Three Brothers Dry Stout', 4.5, 12, 30,
        'A rich and creamy stout with hints of coffee and chocolate.', @BrowarTrzechKumpliId, @DryStoutStyleId,
        'Water, malted barley, oats, roasted barley, hops, yeast',
        DATEADD(day, ABS(CHECKSUM(NEWID())) % 8646, '2000-01-01')),
       (NEWID(), 'Golden Hops German Pilsner', 5.0, 11, 35,
        'A refreshing and crisp German pilsner with a pronounced hop character.', @BrowarTrzechKumpliId,
        @GermanPilsnerStyleId, 'Water, malted barley, hops, yeast',
        DATEADD(day, ABS(CHECKSUM(NEWID())) % 8646, '2000-01-01')),
       (NEWID(), 'White Horse White IPA', 6.0, 14, 40,
        'A hoppy and citrusy IPA with a smooth and creamy mouthfeel.', @BrowarTrzechKumpliId, @WhiteIPAStyleId,
        'Water, malted barley, wheat, oats, hops, yeast', DATEADD(day, ABS(CHECKSUM(NEWID())) % 8646, '2000-01-01')),
       (NEWID(), 'American Dream Pale Ale', 5.5, 12, 35,
        'A classic American pale ale with a balanced malt and hop profile.', @BrowarTrzechKumpliId,
        @AmericanPaleAleStyleId, 'Water, malted barley, hops, yeast',
        DATEADD(day, ABS(CHECKSUM(NEWID())) % 8646, '2000-01-01')),
       (NEWID(), 'Brooklyn Bridge American Porter', 6.5, 15, 45,
        'A rich and full-bodied American porter with notes of chocolate and roasted coffee.', @BrowarTrzechKumpliId,
        @AmericanPorterStyleId, 'Water, malted barley, oats, hops, yeast',
        DATEADD(day, ABS(CHECKSUM(NEWID())) % 8646, '2000-01-01'));

-- Nepomucen brewery
DECLARE @BrowarNepomucenId NVARCHAR(36) = '57e65f98-51cb-4a60-92e7-db5c80f3a157';

INSERT INTO Beers (Id, Name, AlcoholByVolume, Blg, Ibu, Description, BreweryId, BeerStyleId, Composition, ReleaseDate)
VALUES (NEWID(), 'Dark Descent', 11.0, 25.0, 90, 'Imperial stout brewed with cocoa nibs and vanilla',
        @BrowarNepomucenId, @ImperialStoutStyleId, 'water, barley malt, oats, hops, yeast, cocoa nibs, vanilla',
        DATEADD(day, ABS(CHECKSUM(NEWID())) % 8646, '2000-01-01')),
       (NEWID(), 'Pils Italiano', 5.5, 12.0, 35, 'Crisp and refreshing Italian-style pilsner', @BrowarNepomucenId,
        @ItalianPilsnerStyleId, 'water, barley malt, hops, yeast',
        DATEADD(day, ABS(CHECKSUM(NEWID())) % 8646, '2000-01-01')),
       (NEWID(), 'Wheat Walker', 4.8, 12.5, 20, 'American wheat beer brewed with citra and amarillo hops',
        @BrowarNepomucenId, @AmericanWheatStyleId, 'water, wheat malt, barley malt, hops, yeast',
        DATEADD(day, ABS(CHECKSUM(NEWID())) % 8646, '2000-01-01')),
       (NEWID(), 'Kristallklar', 5.0, 11.5, 18, 'Crisp and clear German-style kristallweizen', @BrowarNepomucenId,
        @KristallweizenStyleId, 'water, wheat malt, barley malt, hops, yeast',
        DATEADD(day, ABS(CHECKSUM(NEWID())) % 8646, '2000-01-01')),
       (NEWID(), 'Fruitful Dubbel', 7.0, 18.5, 30, 'Belgian-style dubbel brewed with cherries and plums',
        @BrowarNepomucenId, @FruitDubbelStyleId, 'water, barley malt, wheat malt, hops, yeast, cherries, plums',
        DATEADD(day, ABS(CHECKSUM(NEWID())) % 8646, '2000-01-01'));

-- Maryensztadt brewery
DECLARE @BrowarMaryensztadtId NVARCHAR(36) = '7d9dafe0-4616-4ea7-9540-346093f9cb23';

INSERT INTO Beers (Id, Name, AlcoholByVolume, Blg, Ibu, Description, BreweryId, BeerStyleId, Composition, ReleaseDate)
VALUES (NEWID(), 'Maryensztadt Dubbel IPA', 7.5, 18, 80, N'Double IPA z dodatkiem suszonych owoców',
        @BrowarMaryensztadtId, @DubbelIPAStyleId,
        N'Woda, słód jęczmienny, chmiel, drożdże', DATEADD(day, ABS(CHECKSUM(NEWID())) % 8646, '2000-01-01')),
       (NEWID(), 'Maryensztadt Mild Ale', 3.5, 12, 25, N'Lekkie i łatwe w piciu, z nutą karmelu',
        @BrowarMaryensztadtId, @MildAleStyleId,
        N'Woda, słód jęczmienny, chmiel, drożdże', DATEADD(day, ABS(CHECKSUM(NEWID())) % 8646, '2000-01-01')),
       (NEWID(), 'Maryensztadt American Wild Ale', 6, 16, 45, N'Kwaśne i dzikie piwo, lekko owocowe',
        @BrowarMaryensztadtId, @AmericanWildAleStyleId,
        N'Woda, słód jęczmienny, chmiel, drożdże, bakterie mlekowe',
        DATEADD(day, ABS(CHECKSUM(NEWID())) % 8646, '2000-01-01')),
       (NEWID(), 'Maryensztadt American Hefeweizen', 5, 14, 20,
        N'Klasyczne hefeweizen z dodatkiem amerykańskiego chmielu', @BrowarMaryensztadtId, @AmericanHefeweizenStyleId,
        N'Woda, słód pszeniczny, chmiel, drożdże', DATEADD(day, ABS(CHECKSUM(NEWID())) % 8646, '2000-01-01')),
       (NEWID(), 'Maryensztadt Smoked Porter', 6.5, 15, 40, N'Piwo o smaku wędzonego sera, idealne do mięs',
        @BrowarMaryensztadtId, @SmokedPorterStyleId,
        N'Woda, słód jęczmienny, słód wędzony, chmiel, drożdże',
        DATEADD(day, ABS(CHECKSUM(NEWID())) % 8646, '2000-01-01'));

-- Zakładowy brewery
DECLARE @BrowarZakładowyId NVARCHAR(36) = 'd27c26e4-2e68-42e6-804c-6a190e7d8e9d';

INSERT INTO Beers (Id, Name, AlcoholByVolume, Blg, Ibu, Description, BreweryId, BeerStyleId, Composition, ReleaseDate)
VALUES (NEWID(), 'Lambic', 5.0, 12.0, 10,
        'Traditional Belgian sour beer, spontaneously fermented with wild yeast and bacteria', @BrowarZakładowyId,
        @GueuzeStyleId, 'Malted barley, unmalted wheat, aged hops',
        DATEADD(day, ABS(CHECKSUM(NEWID())) % 8646, '2000-01-01')),
       (NEWID(), 'Flanders Red Ale', 6.0, 14.0, 20,
        'Belgian ale known for its complex fruity and acidic character, with hints of oak and caramel',
        @BrowarZakładowyId, @FlandersRedAleStyleId, 'Malted barley, wheat, crystal malt, roasted barley, aged hops',
        DATEADD(day, ABS(CHECKSUM(NEWID())) % 8646, '2000-01-01')),
       (NEWID(), 'Dunkelweizen', 5.5, 12.5, 15,
        'Dark wheat beer with a malty sweetness and banana and clove esters from the yeast', @BrowarZakładowyId,
        @DunkelweizenStyleId, 'Malted wheat, barley, caramel malt, chocolate malt, Munich malt, noble hops',
        DATEADD(day, ABS(CHECKSUM(NEWID())) % 8646, '2000-01-01')),
       (NEWID(), 'English Porter', 6.2, 13.5, 25,
        'Dark, rich beer with notes of chocolate and coffee, and a smooth, creamy mouthfeel', @BrowarZakładowyId,
        @EnglishPorterStyleId, 'Malted barley, roasted barley, crystal malt, black malt, Fuggle hops',
        DATEADD(day, ABS(CHECKSUM(NEWID())) % 8646, '2000-01-01')),
       (NEWID(), 'English Pale Ale', 5.0, 11.0, 30,
        'Classic British ale with a balance of malt and hop flavors, and a slightly fruity character from the yeast',
        @BrowarZakładowyId, @EnglishPaleAleStyleId, 'Malted barley, crystal malt, English hops, English ale yeast',
        DATEADD(day, ABS(CHECKSUM(NEWID())) % 8646, '2000-01-01'));

-- InneBeczki brewery
DECLARE @BrowarInneBeczkiId NVARCHAR(36) = 'cb613697-073a-4cad-bc27-8d606b49e5c7';

INSERT INTO Beers (Id, Name, AlcoholByVolume, Blg, Ibu, Description, BreweryId, BeerStyleId, Composition, ReleaseDate)
VALUES (NEWID(), 'Foreign Extra Stout', 7.5, 18.5, 50,
        'This full-bodied stout has a rich, roasted flavor with notes of chocolate and coffee. It finishes with a pleasantly bitter hop bite.',
        @BrowarInneBeczkiId, @ForeignExtraStoutStyleId, 'Water, barley malt, hops, yeast',
        DATEADD(day, ABS(CHECKSUM(NEWID())) % 8646, '2000-01-01')),
       (NEWID(), 'Mexican Pilsner', 5.2, 12, 20,
        'This crisp, refreshing pilsner has a light body and a clean finish. It is lightly hopped and has a subtle corn flavor.',
        @BrowarInneBeczkiId, @MexicanPilsnerStyleId, 'Water, barley malt, corn, hops, yeast',
        DATEADD(day, ABS(CHECKSUM(NEWID())) % 8646, '2000-01-01')),
       (NEWID(), 'Gose', 4.2, 8, 10,
        'This tart and slightly salty beer is brewed with coriander and sea salt. It has a light body and a dry finish.',
        @BrowarInneBeczkiId, @GoseStyleId, 'Water, barley malt, wheat, coriander, sea salt, hops, yeast',
        DATEADD(day, ABS(CHECKSUM(NEWID())) % 8646, '2000-01-01')),
       (NEWID(), 'Belgian Pale Ale', 6.5, 14, 35,
        'This complex ale has a fruity, spicy aroma and a malt-forward flavor. It is moderately hopped and has a dry finish.',
        @BrowarInneBeczkiId, @BelgianPaleAleStyleId, 'Water, barley malt, wheat, oats, hops, yeast',
        DATEADD(day, ABS(CHECKSUM(NEWID())) % 8646, '2000-01-01')),
       (NEWID(), 'Robust Porter', 6.0, 15.5, 40,
        'This rich and creamy porter has a chocolatey flavor with hints of caramel and roasted nuts. It finishes with a moderately bitter hop bite.',
        @BrowarInneBeczkiId, @RobustPorterStyleId, 'Water, barley malt, caramel malt, roasted barley, hops, yeast',
        DATEADD(day, ABS(CHECKSUM(NEWID())) % 8646, '2000-01-01'));

-- Profesja brewery
DECLARE @BrowarProfesjaId NVARCHAR(36) = 'b23f45e1-ca48-41b1-9260-7df386086904';

INSERT INTO Beers (Id, Name, AlcoholByVolume, Blg, Ibu, Description, BreweryId, BeerStyleId, Composition, ReleaseDate)
VALUES (NEWID(), 'Smoked Dubbel', 8.5, 18.2, 25, 'Belgian Dubbel with smoky notes', @BrowarProfesjaId,
        @SmokedDubbelStyleId, 'water, malt, hops, yeast, smoked malt',
        DATEADD(day, ABS(CHECKSUM(NEWID())) % 8646, '2000-01-01')),
       (NEWID(), 'American Brown Ale', 5.5, 14, 40, 'Rich and malty with a hoppy finish', @BrowarProfesjaId,
        @AmericanBrownAleStyleId, 'water, malt, hops, yeast',
        DATEADD(day, ABS(CHECKSUM(NEWID())) % 8646, '2000-01-01')),
       (NEWID(), 'Session Pale Ale', 4.5, 12.5, 35, 'Easy drinking pale ale with a light body and low bitterness',
        @BrowarProfesjaId, @SessionPaleAleStyleId, 'water, malt, hops, yeast',
        DATEADD(day, ABS(CHECKSUM(NEWID())) % 8646, '2000-01-01')),
       (NEWID(), 'Belgian Witbier', 4.2, 11.5, 20, 'Light and refreshing with notes of orange and coriander',
        @BrowarProfesjaId, @BelgianWitbierStyleId, 'water, malt, hops, yeast, wheat, orange peel, coriander',
        DATEADD(day, ABS(CHECKSUM(NEWID())) % 8646, '2000-01-01')),
       (NEWID(), 'Extra Pale Ale', 5.8, 14, 50, 'Hoppy and aromatic with a clean finish', @BrowarProfesjaId,
        @ExtraPaleAleStyleId, 'water, malt, hops, yeast', DATEADD(day, ABS(CHECKSUM(NEWID())) % 8646, '2000-01-01'));

-- Spółdzielczy brewery
DECLARE @BrowarSpółdzielczyId NVARCHAR(36) = '155197e5-24f8-4861-8b3e-c6c91247eacf';

INSERT INTO Beers (Id, Name, AlcoholByVolume, Blg, Ibu, Description, BreweryId, BeerStyleId, Composition, ReleaseDate)
VALUES (NEWID(), 'Weizenbock', 8.5, 18, 30, 'Weizenbock to mocniejsza wersja niemieckiego pszenicznego piwa.',
        @BrowarSpółdzielczyId, @WeizenbockStyleId,
        N'Woda, słód pszeniczny, słód jęczmienny, chmiel, drożdże',
        DATEADD(day, ABS(CHECKSUM(NEWID())) % 8646, '2000-01-01')),
       (NEWID(), 'Belgian Dubbel', 7.0, 16, 25, N'Belgijskie piwo o bogatej słodowości i nutach ciemnych owoców.',
        @BrowarSpółdzielczyId, @BelgianDubbelStyleId,
        N'Woda, słód jęczmienny, chmiel, drożdże', DATEADD(day, ABS(CHECKSUM(NEWID())) % 8646, '2000-01-01')),
       (NEWID(), 'Northern English Brown Ale', 4.5, 12, 20,
        N'Tradycyjne angielskie piwo o delikatnej słodowości i nutach orzechów.', @BrowarSpółdzielczyId,
        @NorthernEnglishBrownAleStyleId,
        N'Woda, słód jęczmienny, chmiel, drożdże', DATEADD(day, ABS(CHECKSUM(NEWID())) % 8646, '2000-01-01')),
       (NEWID(), 'Berliner Weisse', 3.0, 8, 5, N'Lekkie, kwaśne niemieckie piwo pszeniczne.', @BrowarSpółdzielczyId,
        @BerlinerWeisseStyleId,
        N'Woda, słód pszeniczny, chmiel, drożdże, bakterie fermentacji mlekowej',
        DATEADD(day, ABS(CHECKSUM(NEWID())) % 8646, '2000-01-01')),
       (NEWID(), 'German Hefeweizen', 5.0, 12, 15,
        N'Klasyczne, niemieckie piwo pszeniczne o intensywnych bananowych i goździkowych aromatach.',
        @BrowarSpółdzielczyId, @GermanHefeweizenStyleId,
        N'Woda, słód pszeniczny, słód jęczmienny, chmiel, drożdże',
        DATEADD(day, ABS(CHECKSUM(NEWID())) % 8646, '2000-01-01')),
       (NEWID(), 'Leffe Brune', 6.5, 17, 20, N'Belgian Dubbel ale brewed by Spółdzielczy brewery.',
        @BrowarSpółdzielczyId, @AbbeyDubbelStyleId, 'Water, malted barley, hops, yeast.',
        DATEADD(day, ABS(CHECKSUM(NEWID())) % 8646, '2000-01-01'));

-- Beer images
INSERT INTO BeerImages (Id, ImageUri, TempImage, BeerId)
SELECT NEWID(), @ImageUri, 1, Id
FROM Beers