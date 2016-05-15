## Swarm Intelligence: The Predator versus Prey Simulation

The simulator comprises of the following components:
* Fifteen fish: represented by small spheroids. 
    * These are physical objects.
    * Have a buoyancy (they can sit suspended in space)
    * Each fish will have a maximum swim radius of 20 meters.
    * A maximum food value that can be used by sharks in satisfying their hunger.
    * Use Separation, Alignment and Cohesion forces for steering, so that fish swim in a school (the parameters for these forces are at your discretion).
    * A dead fish stops functioning as a Boid, turns semitransparent and cannot be detected by sharks.
    * Dead fish become living opaque fish after a period (at you discretion and not greater than 30 seconds).
    * All reliving fish recover their previous living Boid state.
    * Fish will flee from sharks.
* Five sharks: represented by larger spheroids.
    * These are physical objects.
    * Have buoyancy (they can sit suspended in space)
    * Each shark will have a maximum swim radius of 20 meters.
    * A maximum appetite value.
    * Use Separation, Alignment and Cohesion forces for steering (the parameters for these forces are at your discretion). The shark school will not be as cohesive as the fish school.
    * Each shark will use Blumberg’s top-up and decay model, where “decay” moves to increase hunger over time up to the maximum appetite and “top-up” moves to deplete hunger with each fish it eats.
    * Sharks will hunt and attack fish.
