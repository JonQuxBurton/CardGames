# Card Games

[Work in Progress]

A simple Card Shedding game developed as TDD and software design practice.

## Rules
https://bicyclecards.com/how-to-play/crazy-eights/


## Concepts

Turn
Action

## Actions
Play
Take
SelectSuit

## Events
Play
Take
SelectSuit
Pass
TurnEnd
Won

## Scenarios

### Basic Variant
1. PlaySingle not Eight
1. PlaySingle Eight, SelectSuit
1. Take
1. PlaySingle not Eight, Won
1. PlaySingle Eight, Won

### Olsen Olsen Variant
1. PlayMultiple not Eight
1. PlayMultiple Eight, SelectSuit
1. Take, PlayMultiple
1. Take, Take, PlayMultiple
1. Take, Take, Take, Pass
1. PlayMultiple not Eight, Won
1. PlayMultiple Eight, Won




## Design Decisions Log

##### 31/12/2020
[1] Introduce Turn type. Game will now return it from GetCurrentTurn(), rather than having properties, such as DiscardPile.

##### 04/01/2021
[2] Introduce Player type - holds Hand. Then can can have a CurrentPlayer pointer in the Game class which simplifies Play(). Will also be useful for implementing Win functionality later.

##### 05/01/2021
[3] Add GameSetup concept. Game runs the game, while GameSetup sets it up. Simplifies testing for Win functionality, also seperation of concerns.

[4] Add Board concept. Replacing GameSetup. Has low level methods for moving Cards around.

##### 09/01/2021
[5] Add Events for MoveDiscardPileToStock Pile. This allows visibility, making it testable.

##### 13/01/2021
[6] Add DiscardPile, StockPile types to make what these can do more explicit. Such as DiscardPile has a CardToMatch and RestOfCards.

