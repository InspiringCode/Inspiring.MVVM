﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Inspiring.Mvvm {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class ExceptionTexts {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ExceptionTexts() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Inspiring.Mvvm.ExceptionTexts", typeof(ExceptionTexts).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This configuration does not contain the specified behavior &apos;{0}&apos;..
        /// </summary>
        internal static string BehaviorNotContainedByConfiguration {
            get {
                return ResourceManager.GetString("BehaviorNotContainedByConfiguration", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not find a behavior of type &apos;{0}&apos; in the chain of behaviors. Make sure that the list contains a behavior of that type before calling this operation or add a behavior of that type before..
        /// </summary>
        internal static string BehaviorNotFound {
            get {
                return ResourceManager.GetString("BehaviorNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The behavior &apos;{0}&apos; ist not registered. Make sure you specified the exact type of an already registered behavior..
        /// </summary>
        internal static string BehaviorNotRegistered {
            get {
                return ResourceManager.GetString("BehaviorNotRegistered", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot register or search for a closed generic type. Pass &apos;typeof(ExampleBehavior&lt;,&gt;)&apos; instead of &apos;typeof(ExampleBehavior&lt;A, B&gt;)&apos;..
        /// </summary>
        internal static string CannotPassClosedTypeBehavior {
            get {
                return ResourceManager.GetString("CannotPassClosedTypeBehavior", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The value of an empty property path cannot be set. Make sure the path contains at least one property access like &apos;x =&gt; x.Address&apos;..
        /// </summary>
        internal static string CannotSetValueOfEmptyPropertyPath {
            get {
                return ResourceManager.GetString("CannotSetValueOfEmptyPropertyPath", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The value of a &apos;VMCollectionProperty&apos; cannot be set because it is created and managed by the view model framework. Use the &apos;VMCollection&apos; methods to modify the content of the collection..
        /// </summary>
        internal static string CannotSetVMCollectionProperties {
            get {
                return ResourceManager.GetString("CannotSetVMCollectionProperties", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The &apos;VMCollection&lt;TItemVM&gt;&apos; cannot be modified. To support modifications, the source collection to which the &apos;VMCollection&apos; is mapped must implement &apos;IList&lt;T&gt;&apos;..
        /// </summary>
        internal static string CollectionSourceDoesNotImplementListInterface {
            get {
                return ResourceManager.GetString("CollectionSourceDoesNotImplementListInterface", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot set the attached property &apos;View.Model&apos; to an instance of type &apos;{0}&apos; because no view implementation is registered that implements &apos;IView&lt;T&gt;&apos; where T is &apos;{0}&apos; or a base type . Make sure you assigned the correct value or use your dependency injection container to register an appropriate view..
        /// </summary>
        internal static string CouldNotResolveView {
            get {
                return ResourceManager.GetString("CouldNotResolveView", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not set the display value of the &apos;VMProperty&apos; &apos;{0}&apos; because the value has the wrong type. Make sure that you only set it to values that are compatible with &apos;{1}&apos; our use an &apos;IVMValueConverter&apos; with the property that does the conversion. The orignal value was: &apos;{2}&apos;..
        /// </summary>
        internal static string DisplayValueHasWrongType {
            get {
                return ResourceManager.GetString("DisplayValueHasWrongType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Fields are not supported in property paths. Make sure you only reference properties..
        /// </summary>
        internal static string ExpressionCannotContainFields {
            get {
                return ResourceManager.GetString("ExpressionCannotContainFields", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The given expression consists of more than one property. It must select a single property like &apos;x =&gt; x.Address&apos;..
        /// </summary>
        internal static string ExpressionContainsMoreThanOneProperty {
            get {
                return ResourceManager.GetString("ExpressionContainsMoreThanOneProperty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The given expression does not select a property. It must select a single property like &apos;x =&gt; x.Address&apos;..
        /// </summary>
        internal static string ExpressionContainsNoProperties {
            get {
                return ResourceManager.GetString("ExpressionContainsNoProperties", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No value is currently set for the passed &apos;FieldDefinition&apos;. Either call &apos;SetValue&apos; before &apos;GetValue&apos; or use &apos;GetValueOrDefault&apos;..
        /// </summary>
        internal static string FieldNotSet {
            get {
                return ResourceManager.GetString("FieldNotSet", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The passed field does not belong to the same &apos;FieldDefinitionCollection&apos; as this &apos;FieldValueHolder&apos;. Make sure you only access &apos;FieldDefintion&apos;s that were defined on the same &apos;FieldDefintionCollection&apos; instance that was used to create the &apos;FieldValueHolder&apos;..
        /// </summary>
        internal static string ForeignField {
            get {
                return ResourceManager.GetString("ForeignField", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;GetProperties&apos; with an &apos;attributes&apos; parameter to filter the returned properties is not supported..
        /// </summary>
        internal static string GetPropertiesWithAttributesIsNotSupport {
            get {
                return ResourceManager.GetString("GetPropertiesWithAttributesIsNotSupport", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The &apos;VMCollection&lt;TItemVM&gt;&apos; cannot be modified. To support modifications (add, remove and move items), the type of the conatained &apos;ViewModel&apos; objects (&apos;TItemVM&apos;) must implement &apos;IHasSourceObject&apos;..
        /// </summary>
        internal static string HasSourceObjectInterfaceNotImplemented {
            get {
                return ResourceManager.GetString("HasSourceObjectInterfaceNotImplemented", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Parameter &apos;component&apos; is not a valid &apos;ViewModel&apos;. Make sure you pass an object that is derived from &apos;ViewModel&apos; to &apos;TypeDescriptor.GetValue&apos; and &apos;TypeDescriptor.SetValue&apos;. The value of &apos;component&apos; was: &apos;{0}&apos;..
        /// </summary>
        internal static string InvalidComponentInstance {
            get {
                return ResourceManager.GetString("InvalidComponentInstance", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The &apos;CommonBehaviors&apos; behavior &apos;{0}&apos; cannot be enabled on the given object..
        /// </summary>
        internal static string InvalidTargetObjectForCommonBehavior {
            get {
                return ResourceManager.GetString("InvalidTargetObjectForCommonBehavior", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The collection contains no &apos;IScreenLifecycle&apos; of type &apos;{0}&apos;. Use &apos;Contains&apos; to check if an &apos;IScreenLifecycle&apos; is contained in a collection..
        /// </summary>
        internal static string LifecycleTypeNotFound {
            get {
                return ResourceManager.GetString("LifecycleTypeNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The collection contains more than one &apos;IScreenLifecycle&apos; of type &apos;{0}&apos;. You can use &apos;Contains&apos; before you add a &apos;IScreenLifecycle&apos; to ensure that only one instance of a certain type is added..
        /// </summary>
        internal static string MoreThanOneLifecycleTypeFound {
            get {
                return ResourceManager.GetString("MoreThanOneLifecycleTypeFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The given &apos;screen&apos; (or one of its parents) is not associated with a &apos;Window&apos;. Make sure you only call this operation for screens that are currently shown..
        /// </summary>
        internal static string NoAssociatedWindow {
            get {
                return ResourceManager.GetString("NoAssociatedWindow", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No service locator is configured. Make sure you call &apos;ServiceLocator.SetServiceLocator&apos; and pass in your implementation of the &apos;IServiceLocator&apos; interface..
        /// </summary>
        internal static string NoServiceLocatorConfigured {
            get {
                return ResourceManager.GetString("NoServiceLocatorConfigured", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot set calculated property because no setter delegate was specified..
        /// </summary>
        internal static string NoSetterDelegate {
            get {
                return ResourceManager.GetString("NoSetterDelegate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Parameter &apos;binderExpression&apos; must be a &apos;BinderExpression&apos; (or subtype)..
        /// </summary>
        internal static string ParameterMustBeABinderExpression {
            get {
                return ResourceManager.GetString("ParameterMustBeABinderExpression", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot set the value of property path &apos;{0}&apos; because the property &apos;{1}&apos; does not have a setter..
        /// </summary>
        internal static string PropertyIsReadonly {
            get {
                return ResourceManager.GetString("PropertyIsReadonly", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The &apos;VMDescriptor&apos; does not define a &apos;VMProperty&apos; with the name &apos;{0}&apos;..
        /// </summary>
        internal static string PropertyNotFound {
            get {
                return ResourceManager.GetString("PropertyNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not get or set the value of the property path &apos;{0}&apos; because &apos;{1}&apos; is null on object &apos;{2}&apos;..
        /// </summary>
        internal static string PropertyStepIsNull {
            get {
                return ResourceManager.GetString("PropertyStepIsNull", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No &apos;ScreenConductor&apos; with a &apos;ScreenId&apos; of &apos;{0}&apos; could be found. Make sure that the current screen hierarchy contains a &apos;ScreenConductor&apos; instance that implements &apos;IIdentifiedScreen&apos; and returns the given id (this only applies if the &apos;screenId&apos; is not null)..
        /// </summary>
        internal static string ScreenConductorNotFound {
            get {
                return ResourceManager.GetString("ScreenConductorNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This operation can only be called for screens that were shown using &apos;IDialogService.Show&apos; method..
        /// </summary>
        internal static string ScreenIsNoDialog {
            get {
                return ResourceManager.GetString("ScreenIsNoDialog", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The given screen is not contained by this conductor..
        /// </summary>
        internal static string ScreenNotContainedByConductor {
            get {
                return ResourceManager.GetString("ScreenNotContainedByConductor", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The passed type must be a non-abstract class that implements &apos;IBehavior&apos;..
        /// </summary>
        internal static string TypeIsNoBehavior {
            get {
                return ResourceManager.GetString("TypeIsNoBehavior", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The given &apos;NotifyCollectionChangedAction&apos; is not supported..
        /// </summary>
        internal static string UnsupportedCollectionChangedAction {
            get {
                return ResourceManager.GetString("UnsupportedCollectionChangedAction", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The given expression does not select a single property. Make sure it has the form &apos;() =&gt; FirstName&apos;..
        /// </summary>
        internal static string UnsupportedParameterlessPropertyExpression {
            get {
                return ResourceManager.GetString("UnsupportedParameterlessPropertyExpression", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The given expression is not a valid property path. It must only select properties like  &apos;x =&gt; x.Address.City&apos;..
        /// </summary>
        internal static string UnsupportedPropertyPathExpression {
            get {
                return ResourceManager.GetString("UnsupportedPropertyPathExpression", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot set the attached property &apos;View.Model&apos; on an object of type &apos;{0}&apos; to an object of type &apos;{1}&apos;. Make sure the value is of a type &apos;T&apos; for which you have registered an &apos;IView&lt;T&gt;&apos; implementation. Also note that this property can only be set on instances of &apos;ContentControl&apos;, &apos;ContentPresenter&apos; and &apos;IView&lt;T&gt;&apos; where T is &apos;{1}&apos; (or a base type)..
        /// </summary>
        internal static string UnsupportedTargetTypeForModelProperty {
            get {
                return ResourceManager.GetString("UnsupportedTargetTypeForModelProperty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The &apos;Window.DialogResult&apos; property was not assigned yet by the view. Make sure you call this method only in &apos;OnRequestClose&apos; and &apos;OnClose&apos;..
        /// </summary>
        internal static string WindowDialogResultNotAssigned {
            get {
                return ResourceManager.GetString("WindowDialogResultNotAssigned", resourceCulture);
            }
        }
    }
}
