# Design - Rummy

Current:

```plantuml
@startuml
skinparam monochrome true
skinparam Padding 10
hide circle
hide members

class "CLI App" as CLIApp
class InGameController
CLIApp --> InGameController

@enduml
```

Refactor to:

```plantuml
@startuml
skinparam monochrome true
skinparam Padding 10
hide circle
hide members

class "CLI App" as CLIApp
class  InGameController
class  Validator
class  Executor

CLIApp --> InGameController
InGameController --> Validator
InGameController --> Executor

note bottom of Executor
    Because there is no concurrency possible,
    Execute will always succeed if Validate succeeds
end note



@enduml
```
