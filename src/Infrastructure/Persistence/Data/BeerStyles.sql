-- Primary Style ids
Declare @IPAPrimaryStyleId NVARCHAR(36) = 'cceb2b04-0ef3-460b-9d79-81e218c0fdfb';
Declare @StoutPrimaryStyleId NVARCHAR(36) = '878a0872-59f1-4ec1-a774-786fbfde0df1';
Declare @PilsnerPrimaryStyleId NVARCHAR(36) = '05445b09-91a0-440b-9da6-17df11834996';
Declare @WitbierPrimaryStyleId NVARCHAR(36) = 'e9ed8aac-ad1e-4c30-a33f-7ae84ee10d38';
Declare @APAPrimaryStyleId NVARCHAR(36) = '28d43461-271f-4dcf-ba83-f465777ff197';
Declare @PorterPrimaryStyleId NVARCHAR(36) = '10f1573a-074d-46b1-b6a6-c54f9de2f41e';
Declare @HefeweizenPrimaryStyleId NVARCHAR(36) = '3c205985-b903-44b9-9633-260dbb1c7841';
Declare @SourAlePrimaryStyleId NVARCHAR(36) = '1580973e-587b-42c4-898b-23ba6c6a64e5';
Declare @BrownAlePrimaryStyleId NVARCHAR(36) = 'f7c5d66d-74a5-4df3-b6be-5d24c3730c2b';
Declare @DubbelPrimaryStyleId NVARCHAR(36) = '172859db-c06f-4576-bd17-21ce864a0d38';

-- IPA Styles
INSERT INTO BeerStyles VALUES
('0f9a4cc3-25a9-41e7-8b50-7b1542824b2c', 'West Coast IPA', 'An American IPA style that originated on the West Coast of the United States.', 'United States', @IPAPrimaryStyleId, null, null, null, null),
('a4cf7d47-b66e-46f3-a1f1-df8b12293031', 'New England IPA', 'An American IPA style that originated in the Northeastern United States.', 'United States', @IPAPrimaryStyleId, null, null, null, null),
('e4b5f90e-6c2a-42ec-99c8-c3c1a8e9fdda', 'Double IPA', 'A high-alcohol American IPA style with a strong hop flavor and aroma.', 'United States', @IPAPrimaryStyleId, null, null, null, null),
('e0dbb10c-9c6b-408c-91e8-35a74d9b0ef4', 'Black IPA', 'An American IPA style with a dark color and a roasted malt flavor.', 'United States', @IPAPrimaryStyleId, null, null, null, null),
('c8eac9bb-45df-43c6-b02d-8ca5ec2bb320', 'Belgian IPA', 'An IPA style that incorporates Belgian yeast strains and sometimes Belgian malts.', 'Belgium', @IPAPrimaryStyleId, null, null, null, null)

-- Stout Styles
INSERT INTO BeerStyles VALUES
('d1afbd9f-3c4e-4df2-bfc7-7c1e97b3b4af', 'Dry Stout', 'A low-alcohol Irish stout style with a dry finish and roasted flavor.', 'Ireland', @StoutPrimaryStyleId, null, null, null, null),
('4634297d-cdd3-4a0d-8ec5-0a4e4c34a84d', 'Sweet Stout', 'An English stout style with a sweet, creamy flavor and a full body.', 'England', @StoutPrimaryStyleId, null, null, null, null),
('ae372e88-5e43-49b4-9d7d-0c2746cdd401', 'Imperial Stout', 'A high-alcohol stout style with a rich, intense flavor and a full body.', 'England', @StoutPrimaryStyleId, null, null, null, null),
('7dd1d5c9-fc70-4b2d-a78a-019d65743d6a', 'Oatmeal Stout', 'An English stout style with a smooth, silky texture and a hint of sweetness from added oats.', 'England', @StoutPrimaryStyleId, null, null, null, null),
('0574e07c-05b7-4c68-a7c3-ef7e871a75a1', 'Foreign Extra Stout', 'A higher-alcohol stout style with a more complex flavor profile than a standard stout.', 'Ireland', @StoutPrimaryStyleId, null, null, null, null)

-- Pilsner Styles
INSERT INTO BeerStyles VALUES
('4fa0b62a-b15d-4752-b7f2-320c31c3c172', 'German Pilsner', 'A pale, crisp lager style with a mild hop flavor and a dry finish.', 'Germany', @PilsnerPrimaryStyleId, null, null, null, null),
('83f6a918-1462-44e6-9466-8d8f03e33e3a', 'Bohemian Pilsner', 'A slightly sweeter pilsner style with a more pronounced hop flavor than German Pilsner.', 'Czech Republic', @PilsnerPrimaryStyleId, null, null, null, null),
('7c4bdc04-6b4c-4012-ae16-d47ba1d5d7c7', 'American Pilsner', 'An American version of the pilsner style that is often brewed with adjuncts like corn or rice.', 'United States', @PilsnerPrimaryStyleId, null, null, null, null),
('bc7b6c72-4f4f-45dd-a7a7-2b1c4cd5f80f', 'Italian Pilsner', 'A pilsner style brewed in Italy that often incorporates local ingredients like chestnuts or rice.', 'Italy', @PilsnerPrimaryStyleId, null, null, null, null),
('db67cf9b-4316-4fa6-8d84-d1a4d977d4f9', 'Mexican Pilsner', 'A light-bodied pilsner style brewed in Mexico that is often served with a wedge of lime.', 'Mexico', @PilsnerPrimaryStyleId, null, null, null, null)

-- Witbier Styles
INSERT INTO BeerStyles VALUES
('00f6aa3d-4d04-438d-a77a-7d198f3f3b6f', 'Belgian Witbier', 'A Belgian wheat beer style brewed with coriander and orange peel.', 'Belgium', @WitbierPrimaryStyleId, null, null, null, null),
('c9f40191-5241-4c31-80f5-40845d1e7e8d', 'American Wheat', 'An American version of the witbier style that often incorporates American hops.', 'United States', @WitbierPrimaryStyleId, null, null, null, null),
('16d799cf-05e9-4069-b7e1-3b1415f8c2b9', 'Gose', 'A German sour wheat beer style brewed with coriander and salt.', 'Germany', @WitbierPrimaryStyleId, null, null, null, null),
('201cc54b-5e99-4756-af29-4b2c2335a9f3', 'White IPA', 'An IPA and witbier hybrid style that incorporates the hop flavor of an IPA with the spice and citrus of a witbier.', 'United States', @WitbierPrimaryStyleId, null, null, null, null)

-- APA Styles
INSERT INTO BeerStyles VALUES
('f2e167c5-06b6-41f4-b4d1-7fdeaa44b30b', 'American Pale Ale', 'An American version of the English pale ale style that typically has a moderate hop flavor and aroma.', 'United States', @APAPrimaryStyleId, null, null, null, null),
('5f7c9e3d-50b8-4c94-9e61-d6af8b09fae9', 'Extra Pale Ale', 'A lighter-bodied American pale ale style with a lower alcohol content and less hop bitterness than a standard pale ale.', 'United States', @APAPrimaryStyleId, null, null, null, null),
('a0ef1dfc-2dbb-43d3-a2e8-28b46ecaa98c', 'Belgian Pale Ale', 'A Belgian ale style with a moderate hop flavor and a fruity, spicy yeast character.', 'Belgium', @APAPrimaryStyleId, null, null, null, null),
('bcb6a232-b6f4-497d-b84a-9d023dd3d063', 'English Pale Ale', 'A British ale style with a moderate hop flavor and a balanced malt profile.', 'United Kingdom', @APAPrimaryStyleId, null, null, null, null),
('68e3b11c-09c2-4842-81e6-c7dbcc6a3aa5', 'Session Pale Ale', 'A low-alcohol American pale ale style with a similar hop flavor and aroma to a standard pale ale.', 'United States', @APAPrimaryStyleId, null, null, null, null)

-- Porter Styles
INSERT INTO BeerStyles VALUES
('5b5a29c8-98f6-482c-bc50-5c1f4a8005e1', 'English Porter', 'A dark, malty British beer style with flavors of chocolate and caramel.', 'United Kingdom', @PorterPrimaryStyleId, null, null, null, null),
('17c8718d-9ec3-4c5f-9c24-5c025a013a10', 'American Porter', 'An American version of the English porter style that often has a stronger hop presence and a more complex malt profile.', 'United States', @PorterPrimaryStyleId, null, null, null, null),
('c8da23a3-9d95-48d5-bfb6-5f7e066dd17a', 'Baltic Porter', 'A strong, dark beer style that originated in the Baltic region of Europe and has a rich, malty flavor with notes of dark fruit and chocolate.', 'Poland', @PorterPrimaryStyleId, null, null, null, null),
('f7fa33f2-7f24-45c2-8d53-523f6d6a35cb', 'Robust Porter', 'An American porter style that has a bold, roasty flavor and a higher hop bitterness than a standard porter.', 'United States', @PorterPrimaryStyleId, null, null, null, null),
('482b031d-9c33-4478-bbba-45a026cf8c1d', 'Smoked Porter', 'A dark beer style that is brewed with malt that has been smoked over wood, giving it a distinct smoky flavor.', 'Germany', @PorterPrimaryStyleId, null, null, null, null)

-- Hefeweizen Styles
INSERT INTO BeerStyles VALUES
('2d1dc2a9-44dd-40c3-afbb-6d8712aa5085', 'German Hefeweizen', 'A German wheat beer style that is unfiltered and has a hazy appearance, with flavors of banana and clove.', 'Germany', @HefeweizenPrimaryStyleId, null, null, null, null),
('7e768d2a-7af3-432a-8587-5870c8df032c', 'American Hefeweizen', 'An American version of the German hefeweizen style that often has a cleaner, crisper flavor and a lighter body.', 'United States', @HefeweizenPrimaryStyleId, null, null, null, null),
('ebc02b64-40d5-4a7a-97eb-090bbdd726aa', 'Dunkelweizen', 'A dark German wheat beer style that has a rich, malty flavor with notes of chocolate and caramel.', 'Germany', @HefeweizenPrimaryStyleId, null, null, null, null),
('c4b4e6ed-2f19-4480-913e-1eb0d8363f46', 'Weizenbock', 'A strong, dark German wheat beer style that has a complex, malty flavor with notes of dark fruit and spice.', 'Germany', @HefeweizenPrimaryStyleId, null, null, null, null),
('a2f5c0a6-53c7-4a52-a5d8-73a51a9e6cc4', 'Kristallweizen', 'A clear, filtered German wheat beer style that has a lighter body and a crisper flavor than a traditional hefeweizen.', 'Germany', @HefeweizenPrimaryStyleId, null, null, null, null)

-- Sour Ale Styles
INSERT INTO BeerStyles VALUES
('c69b5b71-5d05-4c5e-a79e-f49c3ab87c62', 'Flanders Red Ale', 'A Belgian ale style that is typically aged for extended periods of time, resulting in a complex, sour flavor with notes of fruit and oak.', 'Belgium', @SourAlePrimaryStyleId, null, null, null, null),
('13f46e45-cacb-4d4a-9e48-c1d8c3ad8202', 'Gueuze', 'A traditional Belgian ale style that is made by blending young and old lambic beers, resulting in a tart, sour flavor.', 'Belgium', @SourAlePrimaryStyleId, null, null, null, null),
('0cbb0e5a-58d1-4ee1-a3e5-0674aaf88975', 'Berliner Weisse', 'A German wheat beer style that is light and refreshing, with a tart, sour flavor and a low alcohol content.', 'Germany', @SourAlePrimaryStyleId, null, null, null, null),
('902f4082-4223-44b1-9e37-bf7d3d9d29e4', 'American Wild Ale', 'An American ale style that is fermented with wild yeast and bacteria, resulting in a sour, funky flavor.', 'United States', @SourAlePrimaryStyleId, null, null, null, null);

-- Brown Ale Styles
INSERT INTO BeerStyles VALUES
('b30034aa-0d0d-4e6e-b6f6-18db7944d3f6', 'English Brown Ale', 'A malty English ale style that is typically medium-bodied with a low to moderate hop flavor and a nutty, caramel finish.', 'United Kingdom', @BrownAlePrimaryStyleId, null, null, null, null),
('6b4e0e9f-ee9c-4d09-bd3b-58489d1425ea', 'American Brown Ale', 'An American ale style that is similar to the English Brown Ale, but with a more pronounced hop flavor and a slightly higher alcohol content.', 'United States', @BrownAlePrimaryStyleId, null, null, null, null),
('4619a1ec-290f-4872-a087-5be8c133eb43', 'Mild Ale', 'A low-alcohol English ale style that is typically dark and malty, with a low hop flavor and a slightly sweet finish.', 'United Kingdom', @BrownAlePrimaryStyleId, null, null, null, null),
('b82eab99-1f08-46d7-bbfd-dc2ca8d868d3', 'Northern English Brown Ale', 'A malty English ale style that is similar to the standard English Brown Ale, but with a drier finish and a slightly more pronounced hop flavor.', 'United Kingdom', @BrownAlePrimaryStyleId, null, null, null, null)

-- Dubbel Styles
INSERT INTO BeerStyles VALUES
('0f7e3a3d-6b16-4d4f-8378-6d19f981c7df', 'Belgian Dubbel', 'A Belgian ale style that is dark and malty, with a complex flavor profile that includes notes of caramel, dark fruit, and spices.', 'Belgium', @DubbelPrimaryStyleId, null, null, null, null),
('6935a2bf-c9bb-4bcf-9b37-1cd89e6f9825', 'Abbey Dubbel', 'A Belgian ale style that is similar to the Belgian Dubbel, but brewed by Trappist or Abbey breweries.', 'Belgium', @DubbelPrimaryStyleId, null, null, null, null),
('bc5b4552-daa1-4e9f-af5c-e1007e9df647', 'Dubbel IPA', 'A hybrid beer style that combines the hoppy flavors of an IPA with the dark, malty flavors of a Belgian Dubbel.', 'Belgium', @DubbelPrimaryStyleId, null, null, null, null),
('f0e5a11c-d6c7-43d9-a0ab-1d27d8c9d4a5', 'Smoked Dubbel', 'A Belgian Dubbel style beer that is brewed with smoked malts, giving it a unique smoky flavor and aroma.', 'Belgium', @DubbelPrimaryStyleId, null, null, null, null),
('19edde2a-af0d-46c9-8c6f-0a091e65b5da', 'Fruit Dubbel', 'A Belgian Dubbel style beer that is brewed with fruit, typically cherry or raspberry, to add a sweet and tart flavor to the beer.', 'Belgium', @DubbelPrimaryStyleId, null, null, null, null)
