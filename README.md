# Sapl for Unity Manual

Refer to [sapl.io](https://sapl.io/) to learn more about the Streaming Attribute Policy Language and Engine.

Sapl for Unity uses the [Lightweight Sapl Server](https://github.com/heutelbeck/sapl-policy-engine/tree/master/sapl-server-lt).

# 1 Getting Started

You can add Sapl for Unity to your project via the Unity Package Manager.
Choose "Add Pagacke from git URL..." and add https://github.com/heutelbeck/sapl-unity.git?path=/unity-sapl-package

![image info](https://github.com/heutelbeck/sapl-unity/unity-sapl/docfx/images/AddPackageFromGitURL.png)

![image info](https://github.com/heutelbeck/sapl-unity/blob/main/unity-sapl/docfx/images/PackageManagerSaplUnity.png)

The Sapl for Unity package includes [Sapl for C#](https://github.com/heutelbeck/sapl-unity.git?path=/csharp-sapl)

# 2 Components

After installing the package, you can find the Sapl Components in Packages/SAPL Unity/Runtime/Components.
Add them to your game object like any other Component.

![image info](https://github.com/heutelbeck/sapl-unity/blob/main/unity-sapl/docfx/images/01.PNG)

The Sapl Components contain the following properties which you can set in Unity Inspector. 
All properties exempt *Decision Type* can be set in code too.

- Decision Type:
	*Once* or *Stream*. Once gets a single decision from Sapl Server which is not updated if the policy changes. 
	Stream causes an update every time a policy changes.
- SubjectString:
	The subject of an Authorization Subscription. If both are set, this is overridden by SubjectGameObject.
	Overrides the subject from [SaplRegistry](#4-SaplRegistry)
- SubjectGameObject:
	Add a GameObject here to use it as subject. To identify it, the name and the tag are used.
	Output to sapl server is {"name":"TestName","tag":"TestTag"}. If tag and name are empty its {"name":"New Game Object","tag":"Untagged"}.
	Overrides SubjectString and the subject from [SaplRegistry](#4-SaplRegistry)
- Action:
	The action of the Authorization Subscription. Overrides the default action of the components.
- Resource:
	The respource of the Authorization Subscription. If not set the name of the gameobject is used.
- Environment:
	The environment of the Authorization Subscription. Default value is the name of the scene. The default value is overridden by 
	the environment of [EnvironmentPoint](#24-EnvironmentPoint) and [SaplRegistry](#4-SaplRegistry)
	A set value overrides the environment of [EnvironmentPoint](#24-EnvironmentPoint) and [SaplRegistry](#4-SaplRegistry).
	
If one of this properties is changed at runtime, a new decision will be fetched.

## 2.1 GameObjectEnforcement

This component is used to secure a whole game object (and its children).

Specific properties:

- On Permit and On Not Permit: 
	for both there are 3 possibilities: 
	- set active/set inactive: Sets the game obect active if last decision is permit and inactive otherwise
	- custom on permit/on not permit: Write a component (or as much as you need) which implements [ISaplExecuteable](#31-ISaplExecuteable).
	- do nothing
- Action: 
	The default value for this component is the name of the on permit-method (SetActive,CustomOnPermit or DoNothing). 
	Can be overridden in inspector.
  
### SetInteractableButton
This is a predefined component for use with GameObjectEnforcement custom on permit/on not permit. It toggles the interactibility of a Button.
  
## 2.2 EventMethodEnforcement

This component is used to secure a method.

Specific properties:

- Execute On Permit	
- Execute On Not Permitted  
- Execute On Decision Changed: 

Add handlers in inspector or add them in code.

![image info](https://github.com/heutelbeck/sapl-unity/blob/main/unity-sapl/docfx/images/03.PNG)
	
- Action: 
	The default value for this component is the name of the first persistent On Permit handler. If there are only runtime listeners its RuntimeAction. 
	Can be overridden in inspector.
	
Methods:

- ExecuteMethod()
	Call this method instead of the one you set for Execute On Permit or set it as e. g. On Click () handler in the inspector. 
	The method checks the last Decision and calls the other method if its permitted. 
	Otherwise the Execute On Not Permitted handler is called (if there is one set).
	
- ExecuteMethodParam(Action<object> onPermit, Action<object> onNotPermit=null, object args=null)
	Use this if you want to execute a method that needs params.

	![image info](https://github.com/heutelbeck/sapl-unity/blob/main/unity-sapl/docfx/images/04.PNG)

## 2.3 ComponentEnforcement

This component enables/disables components of the gameobject if the last decision is PERMIT or not.
The components to be secured must implement [IComponentEnforcement](#32-IComponentEnforcement)

- Action: 
	The default value for this component is Enable Component. 
	Can be overridden in inspector.

## 2.4 EnvironmentPoint

This component is used to provide an environment for all sapl components of the gameobject and all its children.
Just drag it there and enter the environment in inspector or change the Environment property in code.

Specific properties
- Environment:
	Default value is the name of the GameObject of the EnvironmentPoint. Overrides the environment of [SaplRegistry](#4-SaplRegistry)

Methods
- public static EnvironmentPoint? GetEnvironmentPoint(GameObject go)
	Gets the valid EnvironmentPoint for go.
  
# 3 Interfaces

## 3.1 ISaplExecuteable
 
 Must be implemented by components which are used to provide custom actions for [GameObjectEnforcement](#21-GameObjectEnforcement).   
 
Methods:
- void OnPermit()
- void OnNotPermit()

## 3.2 IComponentEnforcement

Marks components for [ComponentEnforcement](#23-ComponentEnforcement).

# 4 SaplRegistry

SaplRegistry is attached to a DontDestroyOnLoad GameObject, so its available in all scenes and provides mutual subject and environment.

![image info](https://github.com/heutelbeck/sapl-unity/blob/main/unity-sapl/docfx/images/05.PNG)

Use the static SaplRegistry.GetRegistry() method to get the instance.

Properties:

- CurrentSubject:
	Returns the current Subject as a Newtonsoft.Json.Linq.JToken.
- CurrentEnvironment:
	Returns the current Environment as string.
	Is overridden by [Environment of EnvironmentPoint](#24-EnvironmentPoint) and set [Environment of Components](#2-Components)

Methods:
- static SaplRegistry GetRegistry():
	Returns the SaplRegistry instance.
- void RegisterSubject(string subject):
	Registers a common string.
- bool RegisterSubject(object subject):
	Tries to register an arbitrary object. Returns true if serialization was succesful and false otherwise.
- JToken RegisterGameObjectSubject(GameObject go):
	Registers a GameObject. To identify it, the name and the tag are used.
	Output to sapl server is {"name":"TestName","tag":"TestTag"}. If tag and name are empty its {"name":"New Game Object","tag":"Untagged"}.
	Returns the JToken of the GameObject.
- string GetSubjectJsonString():
	Returns the current Environment as a Json string.
- void SetEnvironment(string environment):
	Registers an environment.
  
# 5 MonoBehaviour Extension

## 5.1 Using Extensions
Extension is available in every Unity-Script derived by MonoBehaviour.
To use the extension methods, you need to use the namespace in your Script like:

	using Sapl.Components;

## 5.2 Extension Methods

async Task AuthorizeOnceAsync(string resource, string subject, string environment, [CallerMemberName] string? action)

	Returns a async Task which sets the recource (name of a boolean value) to the decision and raises the obligations if defined
	there is no need to await the Task, if you dont want to freeze your excecution.

arguments of the method are:
- recource: name of the boolean variable in your script, can be set with the nameof()-operator
- subject: name of the user
- environment: name of the evironment
- action: the name of the action, can be set with the nameof()-operator, if the extension method is called by a custom method, the methodname of custommethod is set by default 
		
bool IsPermitted(MonoBehaviour monoBehaviour, AuthorizationDecision decision)

	Returns true if the decision is Permit and the Obligations can be raised, otherwise false


AuthorizationPublisher AuthorizeAsStream(string resource, string subject, string environment, [CallerMemberName] string? action)

	Returns an IObserver<AuthorizationDecision> which implemets the INotifyDecisionChanged and the INotifyPropertyChanged interfaces 
	Can be used for one subscription. 
	The AuthorizationPublisher has an implemented event-handling to update the given recource from the decision but can also be used to subscribe to 
	the PropertyChanged-Event for any properties in the MonoBehaviour-Script

arguments of the method are:
- recource: name of the boolean variable in your script, can be set with the nameof()-operator
- subject: name of the user
- environment: name of the evironment
- action: the name of the action, can be set with the nameof()-operator, if the extension method is called by a custom method, the methodname of custommethod is set by default 

MultiAuthorizationSubscription MultiAuthorizationSubscription()

	Returns a MultiAuthorizationSubscription to which many single subscriptions (with an idenfier) can be added by calling
	multiAuthorizationSubscription.AddAuthorizationSubscription(string id,string subject, string recource, string environment)
	

MultiAuthorizationPublisher AuthorizeAsStream(MultiAuthorizationSubscription multiAuthorizationSubscription)

	Returns an IObserver<MultiAuthorizationDecision> which implemets the INotifyDecisionChanged and the INotifyPropertyChanged interfaces 
	Can be used for many subscriptions. 
	The MultiAuthorizationPublisher has an implemented event-handling to update the given recources from the decisions but can also be used to subscribe to 
	the PropertyChanged-Event for any properties in the MonoBehaviour-Script.


# 6 Constraint Handling

In the policies, constraints can be formulated in the form of obligations and advices.
It is the task of the PEP to execute these constraints. A distinction must be made between the following:

- **Obligations** must be executed. If no suitable handler is found for an obligation or if the execution of the handler fails (exception),
	the corresponding decision must be set to deny. A permit only remains valid if all obligations have been successfully executed.
- **Advices** do not have to be executed. Missing handlers or errors in execution have no effect on the decision.

For more information on constraints, please refer to the SAPL documentation: [sapl.io](https://sapl.io/)

## 6.1 Constraint API

In order for user-implemented constraint handlers to be executed by the PEP, they must implement a provider interface.
A total of four different provider interfaces are available:

- **RunnableConstraintHandlerProvider**: With the help of this provider, a C# Action can be executed.
- **JsonConsumerConstraintHandlerProvider**: With the help of this provider, a C# Action<in JToken> can be executed.
- **GameObjectConsumerConstraintHandlerProvider**: With the help of this provider, a C# Action<in GameObject> can be executed (does not return a value).
- **BoolFunctionConstraintHandlerProvider**: With the help of this provider, a C# Func<out Boolean> can be executed.

All provider interfaces force the user to implement the following methods:

bool IsResponsible(JToken constraint)

	A constraint is passed to the method as a Json object. How the content of the object is evaluated is up to the user.
	The evaluation must match the formulation of the constraint in the policy (as a complex object or as a string).
	Returns true if the handler is responsible for this constraint.

Signal GetSignal()

	Returns a value of the type Signal. The time of execution of the constraint handler is specified. There are two types:
	Signal.OnDecision (whenever a new decision is received) and
	Signal.OnExecution (whenever an enforced action is executed/triggered).

xxx GetHandler(JToken constraint)

	Returns the method responsible for the constraint. The type "xxx" depends on the implemented provider.

## 6.2 ConstraintEnforcementService

In order for user-implemented constraint handlers to be captured and executed by the Unity PEP, the C# classes must meet two conditions:

- The class must inherit directly or indirectly from the Unity class MonoBehaviour.
- The class must implement one of the provider interfaces ([Constraint API](##6.1-Constraint API)).
- The MonoBehaviour has to be present in the scene.

The Unity PEP scans the Scene for classes that meet the two requirements and stores them in the ConstraintEnforcementService.
Whenever a new decision is received, the Unity PEP checks whether a constraint handler is available for all formulated obligations
and creates two collections containing all constraint handlers to be executed for the respective times of execution.
If an error occurs, the decision is set to deny. The constraint handlers are called with the following context:

- RunnableConstraintHandler without further context.
- JsonConsumerConstraintHandler with a JToken including all constraints contained in the decision as a JArray.
- GameObjectConsumerConstraintHandler with the GameObject to which the Unity PEP is attached as a component.
- BoolFunctionConstraintHandler without further context. If one of the handler returns false, the decision is set to deny.

## 6.3 Built-in constraint handler

The SAPL Unity package contains built-in constraint handlers:

**Logging**:
This handler logs the timestamp, the subject from the [SaplRegistry](#4-SaplRegistry)
and the message that must be specified in the constraint. The handler can be executed at both times of execution.
	
	advice
	{
		"constraintType": "Logging:OnExecution",
		"message": "Type your message here!"
	}
	
**Translate**: 
This handler moves a simple GameObject. The translation is determined via a "Vector3". The handler can be executed at both times of execution.
	
	obligation
	{
		"constraintType" : "Translate:OnDecision",
		"target" : "DemoButton",
		"dataType": "Vector3",
		"data" : {"value":[5,0,0]}
	}
	
**Rotate**:
This handler rotates a simple GameObject. The translation is determined via a "Vector3" with the addition "Self" or "World".
The handler can be executed at both times of execution.
	
	obligation
	{
		"constraintType" : "Rotate:OnExecution",
		"target" : "DemoButton",
		"dataType": "Vector3:Self",
		"data" : {"value":[0,0,5]}
	}
