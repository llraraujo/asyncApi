using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel;
using System.Reflection;

namespace BooksAPI.Helpers
{
    public class ArrayModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            // Esse model binder só funciona com tipos IEnumerable
            if (!bindingContext.ModelMetadata.IsEnumerableType)
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return Task.CompletedTask;
            }

            // Pega o valor inserirdo através do Value Provider
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName).ToString();

            // se o valor for nulo ou espaço em branco, retorna nulo
            if(string.IsNullOrWhiteSpace(value)) 
            {
                bindingContext.Result = ModelBindingResult.Success(null);
                return Task.CompletedTask;
            }

            // Pega o tipo do enumerador e o seu respectivo conversor
            var elementType = bindingContext.ModelType.GetTypeInfo().GenericTypeArguments[0];
            var converter =  TypeDescriptor.GetConverter(elementType);

            // converte cada item da da lista de valores no tipo enumerável
            var values = value.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                              .Select(x => converter.ConvertFromString(x.Trim()))
                              .ToArray();

            // Cria um array do tipo encontrado, no caso de bookcollections é o tipo "Guid", e seta como o Model
            var typedValues = Array.CreateInstance(elementType, values.Length);
            values.CopyTo(typedValues, 0);
            bindingContext.Model = typedValues;

            // retornar um "successful result", passando o modelo;
            bindingContext.Result = ModelBindingResult.Success(bindingContext.Model);
            return Task.CompletedTask;

        }
    }
}
