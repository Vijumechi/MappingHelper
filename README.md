Welcome to the V3SMappingHelper
It is a simple utility class that helps us map/clone data from one object to another. For Eg: when we get the model from service layer and need to build a view model we could use the class to achieve that. This handles public properties and also nested complex objects (Collections) as long as the public properties and there names are the same in both objects.

Usage Example:

MappingHelper.MapObjects(fromModel,toViewModel);
