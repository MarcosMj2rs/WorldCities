using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
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

		/// <summary>
		/// Nome da coluna de classificação (ou nulo se nenhum estiver definido)
		/// </summary>
		public string SortColumn { get; set; }

		/// <summary>
		/// Ordem de classificação ("ASC", "DESC" ou nulo se nenhum estiver definido)
		/// </summary>
		public string SortOrder { get; set; }

		/// <summary>
		/// Filter Column name (or null if none set)
		/// </summary>
		public string FilterColumn { get; set; }

		/// <summary>
		/// Filter Query string (to be used within the given FilterColumn)
		/// </summary>
		public string FilterQuery { get; set; }
		#endregion

		#region[[CONSTRUTOR]]
		/// <summary>
		/// Construtor privado chamado pelo método CreateAsync
		/// </summary>
		private ApiResult(List<T> data, int count, int pageIndex, int pageSize, string sortColumn, string sortOrder, string filterColumn, string filterQuery)
		{
			Data = data;
			PageIndex = pageIndex;
			PageSize = pageSize;
			TotalCount = count;
			TotalPages = (int)Math.Ceiling(count / (double)pageSize);
			SortColumn = sortColumn;
			SortOrder = sortOrder;
			FilterColumn = filterColumn;
			FilterQuery = filterQuery;
		}
		#endregion

		#region[[METODOS]]
		/// <summary>
		/// Páginas uma fonte IQueryable.
		/// </summary>
		/// <param name="source"> Uma fonte IQueryable de tipo genérico</param>
		/// <param name="pageIndex">Índice de página atual baseado em zero (0 = first page)</param>
		/// <param name="pageSize">O Tamannha atual de cada pagina</param>
		/// <param name="sortColumn">The sorting column name</param>
		/// <param name="sortOrder">The sorting order ("ASC" or "DESC")</param>
		/// <param name="filterColumn">The filtering column name</param>
		/// <param name="filterQuery">The filtering query (value to lookup)</param>
		/// <returns>Um objeto que contém o resultado paginado e todas as informações relevantes da navegação de paginação.</returns>
		public static async Task<ApiResult<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize, string sortColumn = null, string sortOrder = null, string filterColumn = null, string filterQuery = null)
		{
			if(!string.IsNullOrEmpty(filterColumn) && !string.IsNullOrEmpty(filterQuery) && IsValidProperty(filterColumn))
				source = source.Where(string.Format("{0}.Contains(@0)", filterColumn), filterQuery);
				
			var count = await source.CountAsync();

			if(!string.IsNullOrEmpty(sortColumn) && IsValidProperty(sortColumn))
			{
				sortOrder = !string.IsNullOrEmpty(sortOrder) && string.Compare(sortOrder, "ASC", true) == 0 ? "ASC" : "DESC";

				source = source.OrderBy(string.Format("{0} {1}", sortColumn, sortOrder));
			}

			source = source.Skip(pageIndex * pageSize).Take(pageSize);

			var data = await source.ToListAsync();

			return new ApiResult<T>(data, count, pageIndex, pageSize, sortColumn, sortOrder, filterColumn, filterQuery);
		}

		/// Checks if the given property name exists
		/// to protect against SQL injection attacks
		/// </summary>
		public static bool IsValidProperty(string propertyName, bool throwExceptionIfNotFound = true)
		{
			var prop = typeof(T).GetProperty(
				propertyName,
				BindingFlags.IgnoreCase |
				BindingFlags.Public |
				BindingFlags.Instance);

			if(prop == null && throwExceptionIfNotFound)
				throw new NotSupportedException(String.Format("ERROR: Propriedade '{0}' não existe.", propertyName));
			
			return prop != null;
		}
		#endregion
	}
}
