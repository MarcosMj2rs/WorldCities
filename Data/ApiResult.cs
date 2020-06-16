using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorldCities.Data
{
	public class ApiResult<T>
	{
		#region [[PROPRIEDADES]]
		/// <summary>
		/// Os dados do resultado.
		/// </summary>
		public List<T> Data { get; private set; }

		/// <summary>
		/// Índice baseado em zero da página atual.
		/// </summary>
		public int PageIndex { get; private set; }

		/// <summary>
		/// Numero de itens contido em cada pagina.
		/// </summary>
		public int PageSize { get; private set; }

		/// <summary>
		/// Total de itens
		/// </summary>
		public int TotalCount { get; private set; }

		/// <summary>
		/// Total de paginas
		/// </summary>
		public int TotalPages { get; private set; }

		public bool HasPreviousPage
		{
			get
			{
				return (PageIndex > 0);
			}
		}

		/// <summary>
		/// TRUE se a página atual tiver uma página seguinte, FALSE caso contrário
		/// </summary>
		public bool HasNextPage
		{
			get
			{
				return ((PageIndex + 1) < TotalPages);
			}
		}
		#endregion

		#region[[CONSTRUTOR]]
		/// <summary>
		/// Construtor privado chamado pelo método CreateAsync
		/// </summary>
		private ApiResult(List<T> data, int count, int pageIndex, int pageSize)
		{
			Data = data;
			PageIndex = pageIndex;
			PageSize = pageSize;
			TotalCount = count;
			TotalPages = (int)Math.Ceiling(count / (double)pageSize);
		}
		#endregion

		#region[[METODOS]]
		/// <summary>
		/// Páginas uma fonte IQueryable.
		/// </summary>
		/// <param name="source"> Uma fonte IQueryable de tipo genérico</param>
		/// <param name="pageIndex">Índice de página atual baseado em zero (0 = first page)</param>
		/// <param name="pageSize">O Tamannha atual de cada pagina</param>
		/// <returns>Um objeto que contém o resultado paginado e todas as informações relevantes da navegação de paginação.</returns>
		public static async Task<ApiResult<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
		{
			var count = await source.CountAsync();
			source = source.Skip(pageIndex * pageSize)
						   .Take(pageSize);

			var data = await source.ToListAsync();

			return new ApiResult<T>(data, count, pageIndex, pageSize);
		}
		#endregion
	}
}
