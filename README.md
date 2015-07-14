# DVRP
Implementation of the computing cluster.

### Names convention & coding guidelines

We use C# naming convention from the MSDN. 

1. https://msdn.microsoft.com/en-us/library/ms229002(v=vs.110).aspx
2. https://msdn.microsoft.com/en-us/library/ff926074.aspx
3. Issues non-specified by the above:
  * Private class members: `_camelCase`
  * Constable `const`, regardless of visibility: `PascalCase`*
  
_if there is a choice, **always** we choose `const` instead of `static readonly`_

Moreover: a name of a branch must include information about the owner and the functionality it covers. Example below (take attention to the capitalization): 

* _pie/refactoring_
* _leo/serialization_
* _rog/comClientUnitTests_
* _kut/componentRegistration_

# FAQ
