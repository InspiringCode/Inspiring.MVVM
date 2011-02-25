namespace Inspiring.Mvvm.ViewModels.Behaviors {
   public enum BehaviorPosition {
      ChainHeader = int.MaxValue,

      OnTop = 1000000,

      CustomOne = 103000,
      CustomTwo = 102000,
      CustomThree = 101000,
      CustomFour = 100000,

      BeforeCollectionValueCache = 24300,
      CollectionValueCache = 24000,
      AfterCollectionValueCache = 23700,

      BeforeCollectionValidator = 23300,
      CollectionValidator = 23000,
      AfterCollectionValidator = 23700,

      BeforeCollectionPopulator = 22300,
      CollectionPopulator = 22000,
      AfterCollectionPopulator = 21700,

      BeforeCollectionInstanceCache = 21300,
      CollectionInstanceCache = 21000,
      AfterCollectionInstanceCache = 20700,

      BeforeCollectionFactory = 20300,
      CollectionFactory = 20000,
      AfterCollectionFactory = 19700,

      BeforeInvalidDisplayValueCache = 12300,
      InvalidDisplayValueCache = 12000,
      AfterInvalidDisplayValueCache = 11700,

      BeforeDisplayValueValidator = 11300,
      DisplayValueValidator = 11000,
      AfterDisplayValueValidator = 10700,

      BeforeDisplayValueAccessor = 10300,
      DisplayValueAccessor = 10000,
      AfterDisplayValueAccessor = 9700,

      BeforeValidator = 4300,
      Validator = 4000,
      AfterValidator = 3700,

      BeforeDisconnectedValueHolder = 3300,
      DisconnectedValueHolder = 3000,
      AfterDisconnectedValueHolder = 2700,

      BeforePropertyChangedTrigger = 2300,
      PropertyChangedTrigger = 2000,
      AfterPropertyChangedTrigger = 1700,

      BeforeSourceValueAccessor = 1300,
      SourceValueAccessor = 1000,
      AfterSourceValueAccessor = 700,

      OnBottom = 100
   }
}
