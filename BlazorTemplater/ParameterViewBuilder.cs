//using Microsoft.AspNetCore.Components;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Reflection;
//using System.Text;
//using System.Threading.Tasks;

//namespace BlazorTemplater
//{
//	/// <summary>
//	/// Represents a builder for a <see cref="ParameterView"/> which can be
//	/// safely passed to a component of type <typeparamref name="TComponent"/>.
//	/// </summary>
//	/// <typeparam name="TComponent">The component type to build parameters for.</typeparam>
//	public class ParameterViewBuilder<TComponent> where TComponent : IComponent
//	{
//		private const string ChildContent = nameof(ChildContent);
//		private static readonly Type TComponentType = typeof(TComponent);

//		private readonly Dictionary<string, object> parameters = new Dictionary<string, object>(StringComparer.Ordinal);

//		/// <summary>
//		/// Adds the <paramref name="value"/> to the parameter selected with the <paramref name="parameterSelector"/>.
//		/// </summary>
//		/// <typeparam name="TValue">Type of <paramref name="value"/>.</typeparam>
//		/// <param name="parameterSelector">A lambda function that selects the parameter.</param>
//		/// <param name="value">The value to pass to <typeparamref name="TComponent"/>.</param>
//		/// <returns>This <see cref="ParameterViewBuilder{TComponent}"/> so that additional calls can be chained.</returns>
//		public ParameterViewBuilder<TComponent> Add<TValue>(Expression<Func<TComponent, TValue>> parameterSelector, TValue value)
//		{
//			if (value is null)
//				throw new ArgumentNullException(nameof(value));

//			parameters.Add(GetParameterName(parameterSelector), value);
//			return this;
//		}

//		/// <summary>
//		/// Adds the <paramref name="content"/> to the parameter selected with <paramref name="parameterSelector"/>.
//		/// </summary>
//		/// <param name="parameterSelector">A lambda function that selects the parameter.</param>
//		/// <param name="content">The content string to pass to the <see cref="RenderFragment"/>.</param>
//		/// <returns>This <see cref="ParameterViewBuilder{TComponent}"/> so that additional calls can be chained.</returns>
//		public ParameterViewBuilder<TComponent> Add(Expression<Func<TComponent, RenderFragment>> parameterSelector, string content)
//			=> Add(parameterSelector, b => b.AddContent(0, content));

//		/// <summary>
//		/// Builds the <see cref="ParameterView"/> with the parameters added to the builder.
//		/// </summary>
//		/// <returns>The created <see cref="ParameterView"/>.</returns>
//		public ParameterView Build() => ParameterView.FromDictionary(parameters);

//		private static string GetParameterName<TValue>(Expression<Func<TComponent, TValue>> parameterSelector)
//		{
//			if (parameterSelector is null)
//				throw new ArgumentNullException(nameof(parameterSelector));

//			if (!(parameterSelector.Body is MemberExpression memberExpression) || !(memberExpression.Member is PropertyInfo propInfoCandidate))
//				throw new ArgumentException($"The parameter selector '{parameterSelector}' does not resolve to a public property on the component '{typeof(TComponent)}'.", nameof(parameterSelector));

//			var propertyInfo = propInfoCandidate.DeclaringType != TComponentType
//				? TComponentType.GetProperty(propInfoCandidate.Name, propInfoCandidate.PropertyType)
//				: propInfoCandidate;

//			var paramAttr = propertyInfo?.GetCustomAttribute<ParameterAttribute>(inherit: true);

//			if (propertyInfo is null || paramAttr is null)
//				throw new ArgumentException($"The parameter selector '{parameterSelector}' does not resolve to a public property on the component '{typeof(TComponent)}' with a [Parameter] or [CascadingParameter] attribute.", nameof(parameterSelector));

//			return propertyInfo.Name;
//		}
//	}
//}
