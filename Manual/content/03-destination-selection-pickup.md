
\pagebreak

## Destination Selection Pickup

![Destination Selection Pickup](content/res/PickupSmall.png)

**Description**  
One of two alternative destination selection methods. Allows the player to pick up the representation of himself/herself and place it somewhere else in the miniature model. Thereby, a travel destination is selected. The destination has to be confirmed to start the travel phase. This is the recommended destination selection method.

**How to Use in Game**  
<!-- The player has to pinch index and thumb to pickup the red player representation in the WIM. A blue destination representation will be picked up. The red player representation stays in place as the player does not move. The blue destination representation can be placed in the miniature model to indicate the desired target position. Once placed, the player can pickup the blue destination indicator and place it somewhere else. Alternatively, the player can pickup the  -->
The user can instantiate a destination indicator, identical
to the one used with direct selection, by grabbing the red user representation in the WIM. Therefore, the user must pinch the index finger and thumb. The destination indicator will be picked up, while the user’s representation stays in place. The user can then place the destination indicator anywhere in the WIM. To change the orientation, the user must turn his or her hand accordingly. The destination indicator can be dropped and picked up again to change the selected destination or orientation. Alternatively, the user can pull another destination indicator out of the red user  representation in the WIM. In this case, the old destination indicator will disappear. To confirm the destination, the destination indicator in the WIM must be double-tapped.


**Setup**  
Set 'Destination Selection Method' to 'Pickup' in miniature model inspector to enable. Requires no additional setup. This is one of the basic features.

**Configuration**  
No configuration required.

**Input**  
Configure 'Pickup Thumb Button' and 'Pickup Index Button' in the input manager. Additionally, the 'Pickup Thumb Button (Touch)' can be configured. It should be configured to the same mapping as 'Pickup Thumb Button'. This is optional, but strongly advised. Configuring this additional button allows to detect when the player no longer touches the thumb button. This will yield much better results and prevents some visual discrepancies (e.g. the player still holding the object only with the index finger). Currently, only one hand (the right hand by default) can be used at a time.