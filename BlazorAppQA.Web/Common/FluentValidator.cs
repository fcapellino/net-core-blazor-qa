using System;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace BlazorAppQA.Web.Common
{
    public class FluentValidator<TValidator> : ComponentBase where TValidator : IValidator, new()
    {
        private readonly static char[] separators = new[] { '.', '[' };
        private TValidator validator;

        [CascadingParameter]
        private EditContext EditContext { get; set; }

        protected override void OnInitialized()
        {
            validator = new TValidator();
            var messages = new ValidationMessageStore(EditContext);

            // revalidate when any field changes, or if the entire form requests validation
            // (e.g., on submit)

            EditContext.OnFieldChanged += (sender, eventArgs)
                => ValidateModel((EditContext)sender, messages);

            EditContext.OnValidationRequested += (sender, eventArgs)
                => ValidateModel((EditContext)sender, messages);
        }

        private void ValidateModel(EditContext editContext, ValidationMessageStore messages)
        {
            var validationResult = validator.Validate(editContext.Model);
            messages.Clear();
            foreach (var error in validationResult.Errors)
            {
                var fieldIdentifier = ToFieldIdentifier(editContext, error.PropertyName);
                messages.Add(fieldIdentifier, error.ErrorMessage);
            }

            editContext.NotifyValidationStateChanged();
        }

        private static FieldIdentifier ToFieldIdentifier(EditContext editContext, string propertyPath)
        {
            // this method parses property paths like 'someprop.mycollection[123].childprop'
            // and returns a fieldidentifier which is an (instance, propname) pair. for example,
            // it would return the pair (someprop.mycollection[123], "childprop"). it traverses
            // as far into the propertypath as it can go until it finds any null instance.

            var obj = editContext.Model;

            while (true)
            {
                var nextTokenEnd = propertyPath.IndexOfAny(separators);
                if (nextTokenEnd < 0)
                {
                    return new FieldIdentifier(obj, propertyPath);
                }

                var nextToken = propertyPath.Substring(0, nextTokenEnd);
                propertyPath = propertyPath.Substring(nextTokenEnd + 1);

                object newObj;
                if (nextToken.EndsWith("]"))
                {
                    // it's an indexer
                    // this code assumes c# conventions (one indexer named item with one param)
                    nextToken = nextToken.Substring(0, nextToken.Length - 1);
                    var prop = obj.GetType().GetProperty("Item");
                    var indexerType = prop.GetIndexParameters()[0].ParameterType;
                    var indexerValue = Convert.ChangeType(nextToken, indexerType);
                    newObj = prop.GetValue(obj, new object[] { indexerValue });
                }
                else
                {
                    // it's a regular property
                    var prop = obj.GetType().GetProperty(nextToken);
                    if (prop == null)
                    {
                        throw new InvalidOperationException($"Could not find property named {nextToken} on object of type {obj.GetType().FullName}.");
                    }
                    newObj = prop.GetValue(obj);
                }

                if (newObj == null)
                {
                    // this is as far as we can go
                    return new FieldIdentifier(obj, nextToken);
                }

                obj = newObj;
            }
        }
    }
}
