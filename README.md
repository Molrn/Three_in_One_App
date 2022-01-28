# Installing / Launching
Unzip 'ThreeInOneApp_build.zip'
Execute 'ThreeInOneApp_build/ThreeInOneApp.exe'

# Accomplished tasks shortlist
  - File writing and reading
  - Restart with 'R' without problem
  - Animator animations (buttons + bird)
  - Code animation (bird)
  - block player input
  - fading sprites
  - randomly generate levels with increasing difficulty
  - different types of obstacles in Furapi Bird
  - timer management
  - lives management
  - high number of coroutines --> manage multiple events at a time
  - deal with multiple audio clips

# Project Features
## Menu
In the menu, despite what was required, I added two things :
  - a button animation (states normal, highlighted and pressed are animated)
  - a fader which waits until the space video is loaded before fading out and fades in when you press a button

## Apple Catcher
I didn't make much modification to this game despite including it in the highscore system I created for the other games. I'll describe it later on.

## Brick Breaker
### Lives system
3 lives at the beginning, lose one if the ball falls, game over when you lose at 0 life

### Level system  
difficulty rises through the levels using 3 parameters :
  - ball speed  
  - brick spawn probability
  - number of brick lines
Generation : symmetric random levels

### Restart feature
press R to restart at any moment :
  - Destroy previous level
  - create new level
  - reinitialize every variable

### HUD description
In game HUD :
  - Score
  - highscore
  - lives
Message display :
  - Game over
  - lives announcer when a life is lost
  - Level announcer when you get to a new level
  - Highscore when you get it

### Sound
I implemented the given sounds and they work correctly
There are 2 play modes for audio clips :
  - wait until previous sound is finished
  - play instantly and cut the previous sound
Intro, game over and death sounds are played using the first mode to make the game feel smoother.
However, for wall, paddle and brick collision, it is played using the second mode, because as these sounds are very played closely, waiting until they are finished creates delayed reactions which feel really weird.

## Furapi Bird
### Bird animation
I used two methods to animate the bird :
  - An Animator (two sprites animation)
  - through the code
code animation :
  - sprite change + stop animator when death
  - rotation : up when fly, down when fall
Rotation angle increases with the bird's speed --> gives speed feeling

### Background
moving clouds and mountains
Mountains + Clouds :
  - constantly move to the left to give the player the feeling of going right
  - Randomly generated sizes
  - When they reach a despawn point, they are teleported to the right of the screen and randomized
Clouds :
  - Randomly generated speed and height
  - 2 big clouds, 4 small ones

### Obstacles
2 kinds of obstacles : difficult and regular
regular obstacles :
  - two pipes moving towards the player
  - constant speed and height
  - spawn on the right of the screen and get despawned once they're out of the screen
Difficult obstacles are regular obstacles with a twist :
  - Moving : obstacles move up and down
  - Series : spawn multiple close pipes with the same height
  - Stairs : series but pipes have regularly increasing or decreasing height
  - Angles (not in the final project) : oriented pipes according to a random Angles
'Angles' weren't implemented in the final project because pipes didn't move towards the player but perpendicularly to their orientation

### Levels
Obstacles are spawned at randomly generated intervals
When spawned, obstacles have a probability to be difficult, otherwise they are regular
When a difficult obstacle is chosen, it randomly spawns one of the 3 difficult kinds
Every 15 seconds, a level is passed, and some variables are modified to increase the difficulty
  - difficulty : probability of spawning a difficult obstacle
  - Pipe speed (it is connected to the point system : the faster pipes go, the faster the score goes)
  - spawn_interval : represent the time interval in which a random value is selected to spawn an obstacle
  - Series_length : The number of obstacles in a row which can be contained in a series or a stair obstacle
Each of these variable has a limit in order not to make the game impossible to play after a while

### HUD description
Similar to the Brick Breaker HUD but simplified (no lives to deal with)
In game HUD : score, highscore
Message display : Game over, highscore

### Sound
I implemented fly and death sounds from the original flappy bird. Moreover, there is a sound (point win) implemented from Brick Breaker wich is played when you get the highscore.

## Highsore system
I created the file "Assets/Resources/Highscores.txt" in which one the highscore of each game is saved. One line represents one highscore.
2 functions (present in every game) :
  - get_highscore() --> fetch each game's highscore then converts the game's highscore into an into
  - update_highscore() --> replace the highscore in the file
Path didn't exist in the final game build so I had to create it manually

# Things to improve
  - There are no transition screen
  - Addition of a more complex highscore system (names input, etc...)
  - Create an end to Apple Catcher (timer based levels where you have to catch a certain number of apples in a limited time for example, or a lives system)
  - Add a death animation for the bird. I tried doing so but it got in the way of the in-game code animation so I removed it
  - A difficulty choice (easy, medium, hard)
  - Make a more diversified sound design (menu doesn't have sound)

# Project surrender
I wasn't able to compress the entire project directory (some files denied the compression), therefore I only surrendered the assets, a unity package containing the assets, the build directory and this README.md file. If it isn't enough, don't hesitate to contact me : youri.aubry@utbm.fr
