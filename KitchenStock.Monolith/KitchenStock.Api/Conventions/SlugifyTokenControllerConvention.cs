using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace KitchenStock.Api.Conventions;

public class SlugifyControllerTokenConvention : IApplicationModelConvention
{
    private readonly IOutboundParameterTransformer _transformer;

    public SlugifyControllerTokenConvention(IOutboundParameterTransformer transformer)
    {
        _transformer = transformer;
    }

    public void Apply(ApplicationModel application)
    {
        foreach (var controller in application.Controllers)
        {
            controller.RouteValues["controller"] =
                _transformer.TransformOutbound(controller.ControllerName);
        }
    }
}