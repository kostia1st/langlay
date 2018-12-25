### [:arrow_double_down: Download the latest version](https://github.com/estorski/langlay/releases/download/v1.16/Langlay.1.16.msi)

# Langlay
A small & simple tool for switching current input language with a custom hotkey, for Windows.

The tool just hangs somewhere in your RAM and waits for your custom keystroke (Caps Lock by default), and then it changes the current system language to the next one in the queue of currently installed languages (or keyboard layouts if you set it up for it).

Note, changing input language and changing keyboard layout - are different actions for this tool. And you are free to customize the most preferable way of working with all these matters.

Additional features:
+ a customizeable overlay being shown on each input language/layout change;
+ a tooltip (which designates the current input language) being shown next to your mouse cursor whenever you click an input field;
+ the tool can completely block the "default" Caps Lock behavior in order to eliminate any frustration possible because of Caps toggled ON by accident.
 
### Possible issues
+ the language does not get switched: doublecheck the settings, and if you have *Input Simulation* chosen as the *Switch Method*, you will have to check if your system has ANY key sequence set for switching your language and/or layout, otherwise the tool has no way to simulate it;
