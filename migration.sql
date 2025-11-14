IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [Carritos] (
    [Id] int NOT NULL IDENTITY,
    [UsuarioId] nvarchar(max) NOT NULL,
    [FechaCreacion] datetime2 NOT NULL,
    [Total] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_Carritos] PRIMARY KEY ([Id])
);

CREATE TABLE [Combos] (
    [Id] int NOT NULL IDENTITY,
    [Nombre] nvarchar(max) NOT NULL,
    [Descripcion] nvarchar(max) NOT NULL,
    [Precio] decimal(18,2) NOT NULL,
    [ImagenUrl] nvarchar(max) NOT NULL,
    [Disponible] bit NOT NULL,
    CONSTRAINT [PK_Combos] PRIMARY KEY ([Id])
);

CREATE TABLE [Ingredientes] (
    [Id] int NOT NULL IDENTITY,
    [Nombre] nvarchar(max) NOT NULL,
    [Descripcion] nvarchar(max) NOT NULL,
    [PrecioAdicional] decimal(18,2) NOT NULL,
    [Disponible] bit NOT NULL,
    CONSTRAINT [PK_Ingredientes] PRIMARY KEY ([Id])
);

CREATE TABLE [Pedidos] (
    [Id] int NOT NULL IDENTITY,
    [NumeroPedido] nvarchar(max) NOT NULL,
    [UsuarioId] nvarchar(max) NOT NULL,
    [ClienteNombre] nvarchar(max) NOT NULL,
    [Telefono] nvarchar(max) NOT NULL,
    [DireccionEntrega] nvarchar(max) NOT NULL,
    [Notas] nvarchar(max) NOT NULL,
    [FechaPedido] datetime2 NOT NULL,
    [Estado] int NOT NULL,
    [Subtotal] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_Pedidos] PRIMARY KEY ([Id])
);

CREATE TABLE [Productos] (
    [Id] int NOT NULL IDENTITY,
    [Nombre] nvarchar(max) NOT NULL,
    [Descripcion] nvarchar(max) NOT NULL,
    [Precio] decimal(18,2) NOT NULL,
    [ImagenUrl] nvarchar(max) NOT NULL,
    [Disponible] bit NOT NULL,
    [Categoria] int NOT NULL,
    [Discriminator] nvarchar(13) NOT NULL,
    [Bebida_Tamanio] int NULL,
    [EsAlcoholica] bit NULL,
    [Mililitros] int NULL,
    [EsVegetariana] bit NULL,
    [Tamanio] int NULL,
    [TipoSalsa] nvarchar(max) NULL,
    CONSTRAINT [PK_Productos] PRIMARY KEY ([Id])
);

CREATE TABLE [ItemPedidos] (
    [Id] int NOT NULL IDENTITY,
    [PedidoId] int NOT NULL,
    [NombreProducto] nvarchar(max) NOT NULL,
    [Descripcion] nvarchar(max) NOT NULL,
    [PrecioUnitario] decimal(18,2) NOT NULL,
    [Cantidad] int NOT NULL,
    [Notas] nvarchar(max) NOT NULL,
    [Subtotal] decimal(18,2) NOT NULL,
    [ProductoId] int NULL,
    [ComboId] int NULL,
    CONSTRAINT [PK_ItemPedidos] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ItemPedidos_Pedidos_PedidoId] FOREIGN KEY ([PedidoId]) REFERENCES [Pedidos] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [IngredienteHamburguesas] (
    [Id] int NOT NULL IDENTITY,
    [HamburguesaId] int NOT NULL,
    [IngredienteId] int NOT NULL,
    [EsExtra] bit NOT NULL,
    [Orden] int NOT NULL,
    CONSTRAINT [PK_IngredienteHamburguesas] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_IngredienteHamburguesas_Ingredientes_IngredienteId] FOREIGN KEY ([IngredienteId]) REFERENCES [Ingredientes] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_IngredienteHamburguesas_Productos_HamburguesaId] FOREIGN KEY ([HamburguesaId]) REFERENCES [Productos] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [ItemCarritos] (
    [Id] int NOT NULL IDENTITY,
    [CarritoId] int NOT NULL,
    [ProductoId] int NULL,
    [ComboId] int NULL,
    [Cantidad] int NOT NULL,
    [Notas] nvarchar(max) NOT NULL,
    [PrecioUnitario] decimal(18,2) NOT NULL,
    [Subtotal] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_ItemCarritos] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ItemCarritos_Carritos_CarritoId] FOREIGN KEY ([CarritoId]) REFERENCES [Carritos] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_ItemCarritos_Combos_ComboId] FOREIGN KEY ([ComboId]) REFERENCES [Combos] ([Id]),
    CONSTRAINT [FK_ItemCarritos_Productos_ProductoId] FOREIGN KEY ([ProductoId]) REFERENCES [Productos] ([Id])
);

CREATE TABLE [ItemCombos] (
    [Id] int NOT NULL IDENTITY,
    [ComboId] int NOT NULL,
    [ProductoId] int NOT NULL,
    [Cantidad] int NOT NULL,
    CONSTRAINT [PK_ItemCombos] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ItemCombos_Combos_ComboId] FOREIGN KEY ([ComboId]) REFERENCES [Combos] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_ItemCombos_Productos_ProductoId] FOREIGN KEY ([ProductoId]) REFERENCES [Productos] ([Id]) ON DELETE CASCADE
);

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Descripcion', N'Disponible', N'ImagenUrl', N'Nombre', N'Precio') AND [object_id] = OBJECT_ID(N'[Combos]'))
    SET IDENTITY_INSERT [Combos] ON;
INSERT INTO [Combos] ([Id], [Descripcion], [Disponible], [ImagenUrl], [Nombre], [Precio])
VALUES (1, N'Hamburguesa Clásica + Papas + Bebida', CAST(1 AS bit), N'/images/combo-clasico.jpg', N'Combo Clásico', 18.0),
(2, N'Hamburguesa Doble Queso + Papas + Bebida', CAST(1 AS bit), N'/images/combo-doble-queso.jpg', N'Combo Doble Queso', 25.0);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Descripcion', N'Disponible', N'ImagenUrl', N'Nombre', N'Precio') AND [object_id] = OBJECT_ID(N'[Combos]'))
    SET IDENTITY_INSERT [Combos] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Descripcion', N'Disponible', N'Nombre', N'PrecioAdicional') AND [object_id] = OBJECT_ID(N'[Ingredientes]'))
    SET IDENTITY_INSERT [Ingredientes] ON;
INSERT INTO [Ingredientes] ([Id], [Descripcion], [Disponible], [Nombre], [PrecioAdicional])
VALUES (1, N'Carne 100% de res premium', CAST(1 AS bit), N'Carne de res', 0.0),
(2, N'Pan brioche artesanal', CAST(1 AS bit), N'Pan brioche', 0.0),
(3, N'Lechuga fresca crujiente', CAST(1 AS bit), N'Lechuga', 0.0),
(4, N'Tomate natural en rodajas', CAST(1 AS bit), N'Tomate', 0.0),
(5, N'Queso cheddar derretido', CAST(1 AS bit), N'Queso cheddar', 2.0),
(6, N'Tocino crujiente', CAST(1 AS bit), N'Tocino', 3.0),
(7, N'Cebolla caramelizada en su punto', CAST(1 AS bit), N'Cebolla caramelizada', 1.5);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Descripcion', N'Disponible', N'Nombre', N'PrecioAdicional') AND [object_id] = OBJECT_ID(N'[Ingredientes]'))
    SET IDENTITY_INSERT [Ingredientes] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Categoria', N'Descripcion', N'Discriminator', N'Disponible', N'EsVegetariana', N'ImagenUrl', N'Nombre', N'Precio') AND [object_id] = OBJECT_ID(N'[Productos]'))
    SET IDENTITY_INSERT [Productos] ON;
INSERT INTO [Productos] ([Id], [Categoria], [Descripcion], [Discriminator], [Disponible], [EsVegetariana], [ImagenUrl], [Nombre], [Precio])
VALUES (1, 0, N'Hamburguesa clásica con carne, lechuga y tomate', N'Hamburguesa', CAST(1 AS bit), CAST(0 AS bit), N'/images/hamburguesa-clasica.jpg', N'Clásica', 12.0),
(2, 0, N'Hamburguesa con doble carne y queso cheddar', N'Hamburguesa', CAST(1 AS bit), CAST(0 AS bit), N'/images/hamburguesa-doble-queso.jpg', N'Doble Queso', 18.0);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Categoria', N'Descripcion', N'Discriminator', N'Disponible', N'EsVegetariana', N'ImagenUrl', N'Nombre', N'Precio') AND [object_id] = OBJECT_ID(N'[Productos]'))
    SET IDENTITY_INSERT [Productos] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Categoria', N'Descripcion', N'Discriminator', N'Disponible', N'ImagenUrl', N'Nombre', N'Precio', N'Tamanio', N'TipoSalsa') AND [object_id] = OBJECT_ID(N'[Productos]'))
    SET IDENTITY_INSERT [Productos] ON;
INSERT INTO [Productos] ([Id], [Categoria], [Descripcion], [Discriminator], [Disponible], [ImagenUrl], [Nombre], [Precio], [Tamanio], [TipoSalsa])
VALUES (3, 1, N'Papas fritas crujientes', N'Papas', CAST(1 AS bit), N'/images/papas-fritas.jpg', N'Papas Fritas Clásicas', 5.0, 1, N'Kétchup');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Categoria', N'Descripcion', N'Discriminator', N'Disponible', N'ImagenUrl', N'Nombre', N'Precio', N'Tamanio', N'TipoSalsa') AND [object_id] = OBJECT_ID(N'[Productos]'))
    SET IDENTITY_INSERT [Productos] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Categoria', N'Descripcion', N'Discriminator', N'Disponible', N'EsAlcoholica', N'ImagenUrl', N'Mililitros', N'Nombre', N'Precio', N'Bebida_Tamanio') AND [object_id] = OBJECT_ID(N'[Productos]'))
    SET IDENTITY_INSERT [Productos] ON;
INSERT INTO [Productos] ([Id], [Categoria], [Descripcion], [Discriminator], [Disponible], [EsAlcoholica], [ImagenUrl], [Mililitros], [Nombre], [Precio], [Bebida_Tamanio])
VALUES (4, 2, N'Refresco de cola', N'Bebida', CAST(1 AS bit), CAST(0 AS bit), N'/images/coca-cola.jpg', 500, N'Coca Cola', 3.5, 1);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Categoria', N'Descripcion', N'Discriminator', N'Disponible', N'EsAlcoholica', N'ImagenUrl', N'Mililitros', N'Nombre', N'Precio', N'Bebida_Tamanio') AND [object_id] = OBJECT_ID(N'[Productos]'))
    SET IDENTITY_INSERT [Productos] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Categoria', N'Descripcion', N'Discriminator', N'Disponible', N'ImagenUrl', N'Nombre', N'Precio', N'Tamanio', N'TipoSalsa') AND [object_id] = OBJECT_ID(N'[Productos]'))
    SET IDENTITY_INSERT [Productos] ON;
INSERT INTO [Productos] ([Id], [Categoria], [Descripcion], [Discriminator], [Disponible], [ImagenUrl], [Nombre], [Precio], [Tamanio], [TipoSalsa])
VALUES (8, 1, N'Papas con queso cheddar y tocino', N'Papas', CAST(1 AS bit), N'/images/papas-chesse-bacon.jpg', N'Papas con Cheese Bacon', 8.0, 2, N'Queso cheddar');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Categoria', N'Descripcion', N'Discriminator', N'Disponible', N'ImagenUrl', N'Nombre', N'Precio', N'Tamanio', N'TipoSalsa') AND [object_id] = OBJECT_ID(N'[Productos]'))
    SET IDENTITY_INSERT [Productos] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Categoria', N'Descripcion', N'Discriminator', N'Disponible', N'EsAlcoholica', N'ImagenUrl', N'Mililitros', N'Nombre', N'Precio', N'Bebida_Tamanio') AND [object_id] = OBJECT_ID(N'[Productos]'))
    SET IDENTITY_INSERT [Productos] ON;
INSERT INTO [Productos] ([Id], [Categoria], [Descripcion], [Discriminator], [Disponible], [EsAlcoholica], [ImagenUrl], [Mililitros], [Nombre], [Precio], [Bebida_Tamanio])
VALUES (9, 2, N'Jugo natural de naranja', N'Bebida', CAST(1 AS bit), CAST(0 AS bit), N'/images/jugo-naranja.jpg', 400, N'Jugo de Naranja', 4.0, 1);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Categoria', N'Descripcion', N'Discriminator', N'Disponible', N'EsAlcoholica', N'ImagenUrl', N'Mililitros', N'Nombre', N'Precio', N'Bebida_Tamanio') AND [object_id] = OBJECT_ID(N'[Productos]'))
    SET IDENTITY_INSERT [Productos] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Cantidad', N'ComboId', N'ProductoId') AND [object_id] = OBJECT_ID(N'[ItemCombos]'))
    SET IDENTITY_INSERT [ItemCombos] ON;
INSERT INTO [ItemCombos] ([Id], [Cantidad], [ComboId], [ProductoId])
VALUES (1, 1, 1, 1),
(2, 1, 1, 3),
(3, 1, 1, 4),
(4, 1, 2, 2),
(5, 1, 2, 3),
(6, 1, 2, 4);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Cantidad', N'ComboId', N'ProductoId') AND [object_id] = OBJECT_ID(N'[ItemCombos]'))
    SET IDENTITY_INSERT [ItemCombos] OFF;

CREATE INDEX [IX_IngredienteHamburguesas_HamburguesaId] ON [IngredienteHamburguesas] ([HamburguesaId]);

CREATE INDEX [IX_IngredienteHamburguesas_IngredienteId] ON [IngredienteHamburguesas] ([IngredienteId]);

CREATE INDEX [IX_ItemCarritos_CarritoId] ON [ItemCarritos] ([CarritoId]);

CREATE INDEX [IX_ItemCarritos_ComboId] ON [ItemCarritos] ([ComboId]);

CREATE INDEX [IX_ItemCarritos_ProductoId] ON [ItemCarritos] ([ProductoId]);

CREATE INDEX [IX_ItemCombos_ComboId] ON [ItemCombos] ([ComboId]);

CREATE INDEX [IX_ItemCombos_ProductoId] ON [ItemCombos] ([ProductoId]);

CREATE INDEX [IX_ItemPedidos_PedidoId] ON [ItemPedidos] ([PedidoId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251029112749_InitialCreate', N'9.0.0');

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251029115548_FixIngredientesBaseYExtra', N'9.0.0');

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'EsExtra', N'HamburguesaId', N'IngredienteId', N'Orden') AND [object_id] = OBJECT_ID(N'[IngredienteHamburguesas]'))
    SET IDENTITY_INSERT [IngredienteHamburguesas] ON;
INSERT INTO [IngredienteHamburguesas] ([Id], [EsExtra], [HamburguesaId], [IngredienteId], [Orden])
VALUES (1, CAST(0 AS bit), 1, 1, 0),
(2, CAST(0 AS bit), 1, 2, 0),
(3, CAST(0 AS bit), 1, 3, 0),
(4, CAST(0 AS bit), 1, 4, 0),
(5, CAST(1 AS bit), 1, 5, 0),
(6, CAST(1 AS bit), 1, 6, 0),
(7, CAST(1 AS bit), 1, 7, 0),
(9, CAST(0 AS bit), 2, 1, 0),
(10, CAST(0 AS bit), 2, 2, 0),
(11, CAST(0 AS bit), 2, 5, 0),
(12, CAST(0 AS bit), 2, 3, 0),
(14, CAST(1 AS bit), 2, 6, 0),
(15, CAST(1 AS bit), 2, 7, 0),
(16, CAST(1 AS bit), 2, 4, 0);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'EsExtra', N'HamburguesaId', N'IngredienteId', N'Orden') AND [object_id] = OBJECT_ID(N'[IngredienteHamburguesas]'))
    SET IDENTITY_INSERT [IngredienteHamburguesas] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Descripcion', N'Disponible', N'Nombre', N'PrecioAdicional') AND [object_id] = OBJECT_ID(N'[Ingredientes]'))
    SET IDENTITY_INSERT [Ingredientes] ON;
INSERT INTO [Ingredientes] ([Id], [Descripcion], [Disponible], [Nombre], [PrecioAdicional])
VALUES (8, N'Pepinillos encurtidos', CAST(1 AS bit), N'Pepinillos', 0.5),
(9, N'Salsa de la casa', CAST(1 AS bit), N'Salsa especial', 0.0);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Descripcion', N'Disponible', N'Nombre', N'PrecioAdicional') AND [object_id] = OBJECT_ID(N'[Ingredientes]'))
    SET IDENTITY_INSERT [Ingredientes] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'EsExtra', N'HamburguesaId', N'IngredienteId', N'Orden') AND [object_id] = OBJECT_ID(N'[IngredienteHamburguesas]'))
    SET IDENTITY_INSERT [IngredienteHamburguesas] ON;
INSERT INTO [IngredienteHamburguesas] ([Id], [EsExtra], [HamburguesaId], [IngredienteId], [Orden])
VALUES (8, CAST(1 AS bit), 1, 8, 0),
(13, CAST(0 AS bit), 2, 9, 0),
(17, CAST(1 AS bit), 2, 8, 0);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'EsExtra', N'HamburguesaId', N'IngredienteId', N'Orden') AND [object_id] = OBJECT_ID(N'[IngredienteHamburguesas]'))
    SET IDENTITY_INSERT [IngredienteHamburguesas] OFF;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251029120829_ArregloRelaciones', N'9.0.0');

ALTER TABLE [Productos] ADD [PrecioGrande] decimal(18,2) NULL;

ALTER TABLE [Productos] ADD [PrecioMediano] decimal(18,2) NULL;

ALTER TABLE [Productos] ADD [PrecioPequeno] decimal(18,2) NULL;

UPDATE [Productos] SET [PrecioGrande] = 4.5, [PrecioMediano] = 3.5, [PrecioPequeno] = 2.5
WHERE [Id] = 4;
SELECT @@ROWCOUNT;


UPDATE [Productos] SET [PrecioGrande] = 5.5, [PrecioMediano] = 4.0, [PrecioPequeno] = 3.0
WHERE [Id] = 9;
SELECT @@ROWCOUNT;


IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Categoria', N'Descripcion', N'Discriminator', N'Disponible', N'EsAlcoholica', N'ImagenUrl', N'Mililitros', N'Nombre', N'Precio', N'PrecioGrande', N'PrecioMediano', N'PrecioPequeno', N'Bebida_Tamanio') AND [object_id] = OBJECT_ID(N'[Productos]'))
    SET IDENTITY_INSERT [Productos] ON;
INSERT INTO [Productos] ([Id], [Categoria], [Descripcion], [Discriminator], [Disponible], [EsAlcoholica], [ImagenUrl], [Mililitros], [Nombre], [Precio], [PrecioGrande], [PrecioMediano], [PrecioPequeno], [Bebida_Tamanio])
VALUES (10, 2, N'Cerveza artesanal de la casa', N'Bebida', CAST(1 AS bit), CAST(1 AS bit), N'/images/cerveza.jpg', 500, N'Cerveza Artesanal', 8.0, 10.0, 8.0, 6.0, 1);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Categoria', N'Descripcion', N'Discriminator', N'Disponible', N'EsAlcoholica', N'ImagenUrl', N'Mililitros', N'Nombre', N'Precio', N'PrecioGrande', N'PrecioMediano', N'PrecioPequeno', N'Bebida_Tamanio') AND [object_id] = OBJECT_ID(N'[Productos]'))
    SET IDENTITY_INSERT [Productos] OFF;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251029141111_AddBebidaTamanioPrecios', N'9.0.0');

EXEC sp_rename N'[Productos].[Discriminator]', N'TipoProducto', 'COLUMN';

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251030115908_TamanioBebida', N'9.0.0');

ALTER TABLE [Productos] ADD [Bebida_PrecioGrande] decimal(18,2) NULL;

ALTER TABLE [Productos] ADD [Bebida_PrecioMediano] decimal(18,2) NULL;

ALTER TABLE [Productos] ADD [Bebida_PrecioPequeno] decimal(18,2) NULL;

UPDATE [Productos] SET [PrecioGrande] = 6.5, [PrecioMediano] = 5.0, [PrecioPequeno] = 3.5
WHERE [Id] = 3;
SELECT @@ROWCOUNT;


UPDATE [Productos] SET [Bebida_PrecioGrande] = 4.5, [Bebida_PrecioMediano] = 3.5, [Bebida_PrecioPequeno] = 2.5
WHERE [Id] = 4;
SELECT @@ROWCOUNT;


UPDATE [Productos] SET [PrecioGrande] = 10.0, [PrecioMediano] = 8.0, [PrecioPequeno] = 6.0, [Tamanio] = 1
WHERE [Id] = 8;
SELECT @@ROWCOUNT;


UPDATE [Productos] SET [Bebida_PrecioGrande] = 5.5, [Bebida_PrecioMediano] = 4.0, [Bebida_PrecioPequeno] = 3.0
WHERE [Id] = 9;
SELECT @@ROWCOUNT;


UPDATE [Productos] SET [Bebida_PrecioGrande] = 10.0, [Bebida_PrecioMediano] = 8.0, [Bebida_PrecioPequeno] = 6.0
WHERE [Id] = 10;
SELECT @@ROWCOUNT;


INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251030184217_AñadirPapas', N'9.0.0');

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Pedidos]') AND [c].[name] = N'UsuarioId');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Pedidos] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [Pedidos] DROP COLUMN [UsuarioId];

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251103032049_RemoverIdUsuario', N'9.0.0');

DROP TABLE [ItemCarritos];

DROP TABLE [Carritos];

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251104162810_arreglarCarrito', N'9.0.0');

UPDATE [Productos] SET [Precio] = 0.0
WHERE [Id] = 4;
SELECT @@ROWCOUNT;


UPDATE [Productos] SET [Precio] = 0.0
WHERE [Id] = 9;
SELECT @@ROWCOUNT;


UPDATE [Productos] SET [Precio] = 0.0
WHERE [Id] = 10;
SELECT @@ROWCOUNT;


INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251104164534_ArreglarBebidas', N'9.0.0');

UPDATE [Productos] SET [Precio] = 3.5
WHERE [Id] = 4;
SELECT @@ROWCOUNT;


UPDATE [Productos] SET [Precio] = 4.0
WHERE [Id] = 9;
SELECT @@ROWCOUNT;


UPDATE [Productos] SET [Precio] = 8.0
WHERE [Id] = 10;
SELECT @@ROWCOUNT;


INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251104184645_ActualizarPreciosBebidas', N'9.0.0');

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251104185505_intentoArregloBebidas', N'9.0.0');

DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Pedidos]') AND [c].[name] = N'DireccionEntrega');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Pedidos] DROP CONSTRAINT [' + @var1 + '];');
ALTER TABLE [Pedidos] DROP COLUMN [DireccionEntrega];

DECLARE @var2 sysname;
SELECT @var2 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Pedidos]') AND [c].[name] = N'Telefono');
IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [Pedidos] DROP CONSTRAINT [' + @var2 + '];');
ALTER TABLE [Pedidos] DROP COLUMN [Telefono];

ALTER TABLE [Pedidos] ADD [TipoPedido] int NOT NULL DEFAULT 0;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251105124053_PedidoSinDireccion', N'9.0.0');

ALTER TABLE [IngredienteHamburguesas] ADD [esObligatorio] bit NOT NULL DEFAULT CAST(0 AS bit);

UPDATE [IngredienteHamburguesas] SET [esObligatorio] = CAST(1 AS bit)
WHERE [Id] = 1;
SELECT @@ROWCOUNT;


UPDATE [IngredienteHamburguesas] SET [esObligatorio] = CAST(1 AS bit)
WHERE [Id] = 2;
SELECT @@ROWCOUNT;


UPDATE [IngredienteHamburguesas] SET [esObligatorio] = CAST(0 AS bit)
WHERE [Id] = 3;
SELECT @@ROWCOUNT;


UPDATE [IngredienteHamburguesas] SET [esObligatorio] = CAST(0 AS bit)
WHERE [Id] = 4;
SELECT @@ROWCOUNT;


UPDATE [IngredienteHamburguesas] SET [esObligatorio] = CAST(0 AS bit)
WHERE [Id] = 5;
SELECT @@ROWCOUNT;


UPDATE [IngredienteHamburguesas] SET [esObligatorio] = CAST(0 AS bit)
WHERE [Id] = 6;
SELECT @@ROWCOUNT;


UPDATE [IngredienteHamburguesas] SET [esObligatorio] = CAST(0 AS bit)
WHERE [Id] = 7;
SELECT @@ROWCOUNT;


UPDATE [IngredienteHamburguesas] SET [esObligatorio] = CAST(0 AS bit)
WHERE [Id] = 8;
SELECT @@ROWCOUNT;


UPDATE [IngredienteHamburguesas] SET [esObligatorio] = CAST(1 AS bit)
WHERE [Id] = 9;
SELECT @@ROWCOUNT;


UPDATE [IngredienteHamburguesas] SET [esObligatorio] = CAST(1 AS bit)
WHERE [Id] = 10;
SELECT @@ROWCOUNT;


UPDATE [IngredienteHamburguesas] SET [esObligatorio] = CAST(0 AS bit)
WHERE [Id] = 11;
SELECT @@ROWCOUNT;


UPDATE [IngredienteHamburguesas] SET [esObligatorio] = CAST(0 AS bit)
WHERE [Id] = 12;
SELECT @@ROWCOUNT;


UPDATE [IngredienteHamburguesas] SET [esObligatorio] = CAST(0 AS bit)
WHERE [Id] = 13;
SELECT @@ROWCOUNT;


UPDATE [IngredienteHamburguesas] SET [esObligatorio] = CAST(0 AS bit)
WHERE [Id] = 14;
SELECT @@ROWCOUNT;


UPDATE [IngredienteHamburguesas] SET [esObligatorio] = CAST(0 AS bit)
WHERE [Id] = 15;
SELECT @@ROWCOUNT;


UPDATE [IngredienteHamburguesas] SET [esObligatorio] = CAST(0 AS bit)
WHERE [Id] = 16;
SELECT @@ROWCOUNT;


UPDATE [IngredienteHamburguesas] SET [esObligatorio] = CAST(0 AS bit)
WHERE [Id] = 17;
SELECT @@ROWCOUNT;


INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251106122827_ingredientesObligatorios', N'9.0.0');

CREATE TABLE [UsuariosAdmin] (
    [Id] int NOT NULL IDENTITY,
    [Email] nvarchar(max) NOT NULL,
    [PasswordHash] nvarchar(max) NOT NULL,
    [Nombre] nvarchar(max) NOT NULL,
    [Rol] int NOT NULL,
    [Activo] bit NOT NULL,
    [FechaCreacion] datetime2 NOT NULL,
    [UltimoAcceso] datetime2 NULL,
    CONSTRAINT [PK_UsuariosAdmin] PRIMARY KEY ([Id])
);

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Activo', N'Email', N'FechaCreacion', N'Nombre', N'PasswordHash', N'Rol', N'UltimoAcceso') AND [object_id] = OBJECT_ID(N'[UsuariosAdmin]'))
    SET IDENTITY_INSERT [UsuariosAdmin] ON;
INSERT INTO [UsuariosAdmin] ([Id], [Activo], [Email], [FechaCreacion], [Nombre], [PasswordHash], [Rol], [UltimoAcceso])
VALUES (1, CAST(1 AS bit), N'admin@restaurante.com', '2025-01-01T00:00:00.0000000Z', N'Administrador Principal', N'$2a$11$HcrEosJ8vlF5n/y.omjmzOJB0N8EioeE/GV1IE3BxUOyH216dEps2', 0, NULL),
(2, CAST(1 AS bit), N'cocina@restaurante.com', '2025-01-01T00:00:00.0000000Z', N'Usuario Cocina', N'$2a$11$s/OkGzemh4.Gp4epaONeLOsY.zDtgIVJT3TAzkTintVwlUn29gO4i', 1, NULL);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Activo', N'Email', N'FechaCreacion', N'Nombre', N'PasswordHash', N'Rol', N'UltimoAcceso') AND [object_id] = OBJECT_ID(N'[UsuariosAdmin]'))
    SET IDENTITY_INSERT [UsuariosAdmin] OFF;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251107022320_UsuariosLogin', N'9.0.0');

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251107191247_CrearTodoEnPcCande', N'9.0.0');

CREATE TABLE [ComboBebidas] (
    [Id] int NOT NULL IDENTITY,
    [ComboId] int NOT NULL,
    [BebidaId] int NOT NULL,
    [RecargoPequeno] decimal(18,2) NOT NULL,
    [RecargoMediano] decimal(18,2) NOT NULL,
    [RecargoGrande] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_ComboBebidas] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ComboBebidas_Combos_ComboId] FOREIGN KEY ([ComboId]) REFERENCES [Combos] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_ComboBebidas_Productos_BebidaId] FOREIGN KEY ([BebidaId]) REFERENCES [Productos] ([Id]) ON DELETE NO ACTION
);

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'BebidaId', N'ComboId', N'RecargoGrande', N'RecargoMediano', N'RecargoPequeno') AND [object_id] = OBJECT_ID(N'[ComboBebidas]'))
    SET IDENTITY_INSERT [ComboBebidas] ON;
INSERT INTO [ComboBebidas] ([Id], [BebidaId], [ComboId], [RecargoGrande], [RecargoMediano], [RecargoPequeno])
VALUES (1, 4, 1, 1.0, 0.0, -1.0),
(2, 9, 1, 1.5, 0.0, -0.5),
(6, 4, 2, 1.5, 0.0, -1.5),
(7, 9, 2, 2.0, 0.0, -1.0),
(8, 10, 2, 2.0, 0.0, -2.0);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'BebidaId', N'ComboId', N'RecargoGrande', N'RecargoMediano', N'RecargoPequeno') AND [object_id] = OBJECT_ID(N'[ComboBebidas]'))
    SET IDENTITY_INSERT [ComboBebidas] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Categoria', N'Descripcion', N'Disponible', N'EsAlcoholica', N'ImagenUrl', N'Mililitros', N'Nombre', N'Precio', N'Bebida_PrecioGrande', N'Bebida_PrecioMediano', N'Bebida_PrecioPequeno', N'Bebida_Tamanio', N'TipoProducto') AND [object_id] = OBJECT_ID(N'[Productos]'))
    SET IDENTITY_INSERT [Productos] ON;
INSERT INTO [Productos] ([Id], [Categoria], [Descripcion], [Disponible], [EsAlcoholica], [ImagenUrl], [Mililitros], [Nombre], [Precio], [Bebida_PrecioGrande], [Bebida_PrecioMediano], [Bebida_PrecioPequeno], [Bebida_Tamanio], [TipoProducto])
VALUES (11, 2, N'Refresco de cola', CAST(1 AS bit), CAST(0 AS bit), N'/images/pepsi.jpg', 500, N'Pepsi', 3.0, 4.0, 3.0, 2.0, 1, N'Bebida'),
(12, 2, N'Refresco de naranja', CAST(1 AS bit), CAST(0 AS bit), N'/images/fanta.jpg', 500, N'Fanta', 3.0, 4.0, 3.0, 2.0, 1, N'Bebida'),
(13, 2, N'Agua sin gas', CAST(1 AS bit), CAST(0 AS bit), N'/images/agua.jpg', 500, N'Agua Mineral', 2.5, 3.5, 2.5, 1.5, 1, N'Bebida');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Categoria', N'Descripcion', N'Disponible', N'EsAlcoholica', N'ImagenUrl', N'Mililitros', N'Nombre', N'Precio', N'Bebida_PrecioGrande', N'Bebida_PrecioMediano', N'Bebida_PrecioPequeno', N'Bebida_Tamanio', N'TipoProducto') AND [object_id] = OBJECT_ID(N'[Productos]'))
    SET IDENTITY_INSERT [Productos] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'BebidaId', N'ComboId', N'RecargoGrande', N'RecargoMediano', N'RecargoPequeno') AND [object_id] = OBJECT_ID(N'[ComboBebidas]'))
    SET IDENTITY_INSERT [ComboBebidas] ON;
INSERT INTO [ComboBebidas] ([Id], [BebidaId], [ComboId], [RecargoGrande], [RecargoMediano], [RecargoPequeno])
VALUES (3, 11, 1, 1.0, 0.0, -1.0),
(4, 12, 1, 1.0, 0.0, -1.0),
(5, 13, 1, 0.5, 0.0, -0.5),
(9, 11, 2, 1.5, 0.0, -1.5),
(10, 13, 2, 1.0, 0.0, -1.0);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'BebidaId', N'ComboId', N'RecargoGrande', N'RecargoMediano', N'RecargoPequeno') AND [object_id] = OBJECT_ID(N'[ComboBebidas]'))
    SET IDENTITY_INSERT [ComboBebidas] OFF;

CREATE INDEX [IX_ComboBebidas_BebidaId] ON [ComboBebidas] ([BebidaId]);

CREATE UNIQUE INDEX [IX_ComboBebidas_ComboId_BebidaId] ON [ComboBebidas] ([ComboId], [BebidaId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251109004018_BebidasSeleccion', N'9.0.0');

ALTER TABLE [Pedidos] ADD [EstadoPago] int NOT NULL DEFAULT 0;

ALTER TABLE [Pedidos] ADD [FechaPago] datetime2 NULL;

ALTER TABLE [Pedidos] ADD [MetodoPago] int NOT NULL DEFAULT 0;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251109141502_pedidosPagos', N'9.0.0');

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251112134355_ReinicioDB', N'9.0.0');

COMMIT;
GO

