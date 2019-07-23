ElementsExample demonstrates MicroStation API for working with different elements.
 
This example demonstrates the following:
   1: Create new elements.
       1.1: Covers the following elements.
           1.1.1: Shape Element: Uses ShapeElement::CreateShapeElement().
           1.1.2: Multiline Element. Uses MultilineElement::CreateMultilineElement().
       1.2: Elements properties can be set at creation time. The following options are available.
           1.2.1: No properties
           1.2.2: Active properties: Uses ElementPropertyUtils::ApplyActiveSettings.
           1.2.3: Some custom properties: Uses ElementPropertiesSetter class. In this case a single value should be supplied that will be used for Color, Weight and Linestyle.
       1.3: The following key-ins are available:
           1.3.1: ELEMENTSEXAMPLE SHAPE CREATE NONE
           1.3.2: ELEMENTSEXAMPLE SHAPE CREATE ACTIVE
           1.3.3: ELEMENTSEXAMPLE SHAPE CREATE CUSTOM SomeIntegerValue
           1.3.4: ELEMENTSEXAMPLE MULTILINE CREATE NONE
           1.3.5: ELEMENTSEXAMPLE MULTILINE CREATE ACTIVE
           1.3.6: ELEMENTSEXAMPLE MULTILINE CREATE CUSTOM SomeIntegerValue
   2: Copy elements.
       2.1: Any element can be copied. Uses ElementCopyContext::DoCopy().
       2.2: Elements can be copied between models.
           2.2.1: A model name should be specified that will become the destination model for copy operation.
           2.2.2: If no model is specified the element is copied inside active model.
       2.3: The following key-in is available:
           2.3.1: ELEMENTSEXAMPLE ELEMENT COPY SomeModelName
   3: Query elements.
       3.1: Elements can be queried for the following:
           3.1.1: Display or basic properties. This example covers only color, weight and linestyle. Any element can be queried for basic properties. Uses ElementPropertiesGetter class.
           3.1.2: Fill properties i.e. Solid fill, Gradient and Pattern. Any element that supports fill can be queried for fill properties. Uses IAreaFillPropertiesQuery interface.
           3.1.3: Geometric Properties. This example only covers Shape and Multiline elements. Uses ICurvePathQuery for shape element and IMultilineQuery for multiline element.
       3.2: The following key-ins are available:
           3.2.1: ELEMENTSEXAMPLE ELEMENT QUERY DISPLAY
           3.2.2: ELEMENTSEXAMPLE ELEMENT QUERY GEOMETRY
           3.2.3: ELEMENTSEXAMPLE ELEMENT QUERY FILL
   4: Edit elements.
       4.1: The following properties of elements can be changed:
           4.1.1: Display or basic properties. This example covers only color, weight and linestyle. Any element properties can be changed. Uses ElementPropertiesGetter or ElementPropertyUtils class.
           4.1.2: Fill properties i.e. Solid fill, Gradient and Pattern. Properties of any element that supports fill can be changed. Uses IAreaFillPropertiesEdit interface.
           4.1.3: Geometric Properties. This example only covers Shape and Multiline elements. This example only reverses the points of shape or multiline element. Uses ICurvePathEdit for shape element and IMultilineEdit for multiline element.
       4.2: The following key-ins are available:
           4.2.1: ELEMENTSEXAMPLE ELEMENT EDIT DISPLAYDEFAULT: Change basic properties to default values of 0.
           4.2.2: ELEMENTSEXAMPLE ELEMENT EDIT DISPLAYACTIVE : Change basic properties to active settings. This will cover properties other than stated above.
           4.2.3: ELEMENTSEXAMPLE ELEMENT EDIT DISPLAYCUSTOM : Change basic properties to some custom values. This example is using some constant values.
           4.2.4: ELEMENTSEXAMPLE ELEMENT EDIT GEOMETRY      : Change element geometry.
           4.2.5: ELEMENTSEXAMPLE ELEMENT EDIT FILLNONE      : This means remove any fill currently on the element.
           4.2.6: ELEMENTSEXAMPLE ELEMENT EDIT FILLSOLID     : Add solid fill to element.
           4.2.7: ELEMENTSEXAMPLE ELEMENT EDIT FILLPATTERN   : Add pattern fill to element.
           4.2.8: ELEMENTSEXAMPLE ELEMENT EDIT FILLGRADIENT  : Add gradient fill to element.