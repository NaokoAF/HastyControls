# Enhanced controller support for [Haste](https://store.steampowered.com/app/1796470/Haste/)
[![Trailer](http://i.ytimg.com/vi/-OfOumEhUOE/hqdefault.jpg)](https://www.youtube.com/watch?v=-OfOumEhUOE)
### [Install on Workshop](https://steamcommunity.com/sharedfiles/filedetails/?id=3456414574)

## Features
* Gyro Aiming (Motion Controls)
* Rumble support on many game actions (Configurable per action)
* Adjustable auto look (Camera automatically tilting to face where you're going)
* Adjustable deadzones, sensitivity, and power curve of the right stick
* All features highly configurable within the in-game settings
* And more things planned if I find the time

## Controller Support
* **Xbox:** Partial support. Controllers have no Gyro, but other features work perfectly.
* **DualShock 4 (PS4):** Fully supported. What I currently play with
* **DualSense (PS5):** Fully supported.
* **Switch Pro Controller:** Partial support. Menu navigation is broken, but playable.
* **Steam Deck:** Probably, but unknown at the moment, as I don't own one.

## My camera is turning on it's own! (Gyro Drift)
Put your controller down on a flat surface and hold down the calibration button for about a second (click the touchpad by default).
To avoid this next time, leave the controller down while launching the game. It calibrates automatically on every launch.

## Setting Descriptions
* **Gamepad Power Curve:** Right stick power curve. Values above 1 squish motion closer to the center of the stick, values below 1 stretch them closer to the edge.
* **Gamepad/Gyro Sensitivity Ratio:** Vertical sensitivity multiplier. 0.75 means vertical sensitivity is 75% slower than horizontal sensitivity.
* **AutoLook Settings:** Haste automatically moves your camera around to match your movement. You can play around with these or just set them to 0 to disable it.
* **Gyro Space:** Algorithm used to convert real world movements to the in game camera. Player space recommended for most cases, Local for handhelds. Turn and Lean depend on preference, so try both!
* **Gyro Button:** Controller button to enable/disable gyro. Check [this link](https://wiki.libsdl.org/SDL3/SDL_GamepadButton) for more detail on each option.
* **Gyro Button Mode:** Behavior of gyro button. Off disables motion controls while the button is held, On enables them, Toggle switches between on and off on each press.
* **Gyro Calibrate Button:** Button to correct gyro drift.
* **Gyro Tightening:** Soft gyro deadzone to reduce the effects of shaky hands. Values above 6 can start to feel bad.
* **Gyro Smoothing Threshold:** Rotations below this speed are smoothed. Also helps reduce the effects of shaky hands.
* **Gyro Smoothing Time:** How long to smooth for. Higher values add latency.
* **Rumble Intensity:** Overall rumble intensity. 0 to disable rumble completely.
* **Other Rumble Settings:** Rumble intensity per game action. 0 to disable.

## Why?
I'm a big fan of Gyro Aiming, but Haste doesn't support Mixed Input, as in, using the left stick to move while using the mouse to look around. Mixed Input is a requirement to use Steam Input's Gyro to Mouse feature.

That inspired me to make a mod that would simply allow for Mixed Input, but after not being able to make that work, the mod's scope instead quickly escalated to adding gyro controls to the game, later escalating to improved controls in general.

This was also a fun way to play around with what I learned from Jibb Smart's [GyroWiki](http://gyrowiki.jibbsmart.com/), and as a learning experience on modding Unity games.