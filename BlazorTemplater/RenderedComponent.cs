using Microsoft.AspNetCore.Components;

namespace BlazorTemplater
{
    /// <summary>
    /// Rendered instance of a component
    /// </summary>
    /// <typeparam name="TComponent"></typeparam>
    /// <remarks>
    /// Adapted from BlazorUnitTestingPrototype
    /// </remarks>
    internal class RenderedComponent<TComponent> where TComponent : IComponent
    {
        private readonly HtmlRenderer _renderer;
        private readonly ContainerComponent _containerTestRootComponent;
        private int _testComponentId;
        private TComponent _testComponentInstance;

        internal RenderedComponent(HtmlRenderer renderer)
        {
            _renderer = renderer;
            _containerTestRootComponent = new ContainerComponent(_renderer);
        }

        public TComponent Instance => _testComponentInstance;

        public string GetMarkup()
        {
            return Htmlizer.GetHtml(_renderer, _testComponentId);
        }

        internal void SetParametersAndRender(ParameterView parameters)
        {
            _containerTestRootComponent.RenderComponentUnderTest(
                typeof(TComponent), parameters);
            var foundTestComponent = _containerTestRootComponent.FindComponentUnderTest();
            _testComponentId = foundTestComponent.Item1;
            _testComponentInstance = (TComponent)foundTestComponent.Item2;
        }
    }
}