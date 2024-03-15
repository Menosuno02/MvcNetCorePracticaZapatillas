using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MvcNetCorePracticaZapatillas.Data;
using MvcNetCorePracticaZapatillas.Models;
using System.Data;

#region PROCEDURES
/*
CREATE OR ALTER PROCEDURE SP_PAGINACION_ZAPATILLAS
(@POSICION INT, @IDZAPATILLA INT, @REGISTROS INT OUT)
AS
	SELECT @REGISTROS = COUNT(IDIMAGEN)
	FROM IMAGENESZAPASPRACTICA
	WHERE IDPRODUCTO = @IDZAPATILLA
	SELECT IDIMAGEN, IDPRODUCTO, IMAGEN
	FROM
	(
		SELECT CAST(ROW_NUMBER() OVER (ORDER BY IDPRODUCTO) AS int)
		AS POSICION, IDIMAGEN, IDPRODUCTO, IMAGEN
		FROM IMAGENESZAPASPRACTICA
		WHERE IDPRODUCTO = @IDZAPATILLA
	)
	AS QUERY
	WHERE QUERY.POSICION = @POSICION
GO
*/
#endregion

namespace MvcNetCorePracticaZapatillas.Repositories
{
    public class RepositoryZapatillas
    {
        private ZapasContext context;
        public RepositoryZapatillas(ZapasContext context)
        {
            this.context = context;
        }

        public async Task<List<Zapatilla>> GetZapatillasAsync()
        {
            return await this.context.Zapatillas.ToListAsync();
        }

        public async Task<Zapatilla> FindZapatillaAsync(int idzapatilla)
        {
            return await this.context.Zapatillas
                .FirstOrDefaultAsync(z => z.IdProducto == idzapatilla);
        }

        public async Task<PaginacionZapatillas> GetPaginacionZapatillasAsync
            (int posicion, int idzapatilla)
        {
            PaginacionZapatillas model = new PaginacionZapatillas();
            model.Zapatilla = await FindZapatillaAsync(idzapatilla);
            string sql = "SP_PAGINACION_ZAPATILLAS @POSICION, @IDZAPATILLA, @REGISTROS OUT";
            SqlParameter paramPosicion = new SqlParameter("@POSICION", posicion);
            SqlParameter paramZapatilla = new SqlParameter("@IDZAPATILLA", idzapatilla);
            SqlParameter paramRegistros = new SqlParameter("@REGISTROS", -1);
            paramRegistros.Direction = ParameterDirection.Output;
            var consulta = this.context.ImagenesZapatillas
                .FromSqlRaw(sql, paramPosicion, paramZapatilla, paramRegistros);
            List<ImagenZapatilla> imagenes = await consulta.ToListAsync();
            model.Imagen = imagenes.FirstOrDefault();
            model.NumRegistros = (int)paramRegistros.Value;
            return model;
        }

        public async Task<int> GetMaxIdImagen()
        {
            if (await this.context.ImagenesZapatillas.CountAsync() == 0) return 1;
            return await this.context.ImagenesZapatillas.MaxAsync(i => i.IdImagen) + 1;
        }

        public async Task InsertImagenes(List<string> imagenes, int idzapatilla)
        {
            foreach (string imagen in imagenes)
            {
                int idimagen = await GetMaxIdImagen();
                await this.context.ImagenesZapatillas.AddAsync(
                    new ImagenZapatilla
                    {
                        IdImagen = idimagen,
                        IdProducto = idzapatilla,
                        Imagen = imagen
                    }
                );
                await this.context.SaveChangesAsync();
            }
        }
    }
}
