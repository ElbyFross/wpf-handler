# Uniform Data Operator
It's a framework that allow to oparate and manage yours data by unified way, not depending from your data base or prefered format. Standardize your data structures and avoid adjust of your product only for one storage type that could be not suitable for you in future.

# Modules 
## Binnary Handler
Provides base API for binary serizliation process.

## SQLOperatorHandler
Provides methods that simplify converting of app's data to query format.
Inform subscribers about `ISqlPperators` events.
Provides access to current `Active` SQL operator.

## AttributesHandler
Provides API thats simplify working with UDO attributes and members data.

## Included operators:
- `MySQLDataOperator` - provides uniform way to manage your data via MySQL database.

# F.A.Q.
## How to describe table class/struct?
### Conception
At first you need to understand conception of work with data and them representations at database.

For every table on server that must be compatible with your application on native level you need to provide the 
class/structure with compatible and correct described fields/properties.

This class/structure would be a bridge betwee your local data and server representation.

### Describing
Describing of data making by using of attributs from `UniformDataOperator.Sql.Attributes` namespace. 
(Custom attributes and modifiers can has other namespace).

1. Define `Table` attribute for your class/structure. Describe correct scheme and table name.
2. Define `System.Serializable` attribute for your class.
3. For every field/property that would be a column defind `Column` attribute. Set column name and type at constructor.
4. Define additive columns' attributes like `isPrimaryKey`, `isAutoIncrement`, `Commentary`, etc. More details you can wind in source or offline documetation.

## How operator defines what would be included in auto generated read/write queries?
Every operator can has a different algorithm related to specific requirements of database server.
But a common idea is mapping of you 'Table' defined class by `Column` attributes and generate the queries based on them settings and values.

If some attribute affecting algorithm then that described at that's summary.

## I need to manage data received from server. How I can do it?
Just use a property as column. Then you would be able to manage a complex get/set algorithms during operator actions.

## Can I add supporting of other SQL server?
Sure. Just create you ServerNameOperator that implement ISqlOperator interface and your data described by UDO's attributes would be
compatible with your specific server.

Use the default MySqlDataOperator (group of partial classes) as example. Them are pretty good documented.
If you done this job please consider sharing this source as contribution into UDO.

## My SQL server incompatible with DbDataType's indexes. How I can adjust my data?
By default `Column` attribute described via `DbDataType` that mostly unusable for huge count of types that custom for every different SQL server.
If you faced with such kind of problem so just create your own modifying attribute that would be used on your custom `ISqlOperator` instance.

Use `UniformDataOperator.Sql.MySql.Attributes.MySqlDBTypeOverride` as example for your source. 
Check how it using at `MySqlDataOperatorCommands.cs` in  `ColumnDeclarationCommand` method.

## Auto write/read not enough flexible for me. How I can make custom query?
Auto managing of duplex exchanging of data with SQL server it's just a high end feature.
But not the only way to manage your uniformed data.

All what you need it's pipe down on one level and start direct work with your `ISqlOperator` instance.

Make your own SQL query suitable by your specific task and send it to server via `SqlOperatoHandler.Active` by use one of provided methods :
- `ExecuteNonQuery`
- `ExecuteScalar`
- `ExecuteReader`
- `Count`

## How to establish connection with server?
1. Create and instance of your `ISqlOperator` instance. 
2.  Initialize properties of your `ISqlOperator` instance:
	- `string Server`
	- `int Port`
	- `string Database`
	- `string UserId`
	- `string Password`
3. Call `Initialize` method on your `ISqlOperator` instance.
4. Call `OpenConnection` method on your `ISqlOperator` instance.
5. Execute your SQL query.
6. Call `CloseConnection` method on your `ISqlOperator` instance.
