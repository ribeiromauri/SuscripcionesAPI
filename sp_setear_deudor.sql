USE [SuscripcionesAPI]
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SetearDeudor]
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	UPDATE AspNetUsers
	set 
	Deudor = 'True'
	from Facturas
	inner join AspNetUsers
	on AspNetUsers.Id = Facturas.UsuarioId
	where Pagada = 'False' and FechaLimiteDePago < GETDATE()
END
