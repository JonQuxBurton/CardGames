# Project Plan - Rummy

Rules:
https://www.pagat.com/rummy/rummy.html
https://bicyclecards.com/how-to-play/rummy-rum/

## Milestones

- Implement Two Player Game
- Implement Three or More Players Game

## Actions and Events

Action is a Users Action

| Action              | Event                    | Phase   |
| ------------------- | ------------------------ | ------- |
| CreateGame          | GameCreated              | Setup   |
| JoinGame            | PlayerJoined             | Setup   |
| -                   | StartingPlayerChosen     | Setup   |
| -                   | DeckCreated              | Setup   |
| -                   | DeckShuffled             | Setup   |
| -                   | Dealt                    | Setup   |
| -                   | CardsMovedToStockPile    | Setup   |
| -                   | CardMovedToDiscardPile   | Setup   |
| -                   | TurnedUpDiscardPile      | Setup   |
| TakeFromStockPile   | TakenCardFromStockPile   | InGame  |
| TakeFromDiscardPile | TakenCardFromDiscardPile | InGame  |
| -                   | RefreshedStockPile       | InGame  |
| Discard             | DiscardedCard            | InGame  |
| Layoff              | LayedoffCard             | InGame  |
| Laydown             | LayeddownMeld            | InGame  |
| -                   | RoundWon                 | Results |
| -                   | GameWon                  | Results |

### Discard

Validation:
Must be Player's turn
Must have Card in Player's Hand
Player must have previously either Taken, Layoff or Laydown
Cannot Discard Card just drawn

Result:
Card removed from Players Hand
Card added to Discard Pile
Check for Win

### Layoff

Validation:
Must be Player's turn
Must have Cards in Player's Hand
The Cards must match a Meld on the Table

Result:
Card removed from Players Hand
Cards added to Meld
Check for Win

### Laydown

Validation:
Must be Player's turn
Must have Cards in Player's Hand
The Cards must be a Meld

Result:
Cards removed from Players Hand
Meld added to Table
Check for Win

## Deal

| NumberOfPlayers | NumberOfCardsDealt |
| --------------- | ------------------ |
| 2               | 10                 |
| 3               | 7                  |
| 4               | 7                  |
| 5               | 6                  |
| 6               | 6                  |

## Configuration

- NumberOfPlayers (2-6)
- AcesHigh

## Architecture

Core of system seems to match the Command pattern
