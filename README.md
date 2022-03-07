# Readme Generator

A simple application for generating readme files inspired by [readme.so](https://readme.so/)

## Controls

- <kbd>CLICK</kbd> on readme template item to edit it
- <kbd>CLICK</kbd> on all sections item to add section to readme template
- <kbd>TAB</kbd> to select the first placeholder text, or jump to the next section
- <kbd>LEFT CTRL</kbd> + <kbd>TAB</kbd> to go to the previous section
- <kbd>LEFT SHIFT</kbd> + <kbd>CLICK</kbd> on all sections item to edit it
- <kbd>LEFT ALT</kbd> + <kbd>CLICK</kbd> on a list item to delete it

## Roadmap

- [x] snippet mode (press tab to move to the next value, typing changes current placeholder text)
- [x] add custom section templates (user created sections that can be added to a readme file)
- [ ] add custom readme templates (readme templates created from the section templates)
- [ ] maybe allow for section, readme and global variables (once the value is set in snippet mode, all occurences get replaced with the value)
- [x] save section templates as yaml files
- [x] move to the next/prev section functionality
- [x] save readme (as yaml)
- [ ] add window to create a new readme template (and display available readme templates)
- [x] copy readme to clipboard
- [ ] move section up/down
- [x] edit section
- [x] delete section
- [ ] edit readme template
- [ ] delete readme template
- [x] add selection wrappers for the currently selected text (to make text italic, bold, create links etc.)
- [ ] maybe add tab completion for tab triggers
- [ ] maybe add a way to insert emojis
- [ ] maybe add the option to import a templates list file and combine the imported templates with the currently saved ones. It would also be nice to have a "new" tag for recently added section and readme templates
- [ ] maybe change the font and make it a bit larger
- [ ] section window should focus the name field
- [ ] maybe rightclick should remove items from lists
- [x] selecting a section should jump to the first replacement match
- [ ] see if it is possible to show an adorner (like a tab keyboard key image) at the current cursor position when tab expansion is available
- [ ] maybe add a check icon if a section in the selected sections list has no placeholder values in its body
- [x] the readme output should probably not be editable
- [ ] maybe allow creating new readme templates from the main page
- [ ] maybe add a readme importing option that generates a section from each heading and has the text between as a body
- [ ] maybe make the selection wrapped by `<kbd>` tags always be uppercase
- [ ] maybe add a way to declare a basic item for a section (for example, a task item for the roadmap section. Maybe make it so that this basic item gets expanded by tab completion
- [ ] maybe show a list of all the found placeholder values and an input field next to them for their desired replacements
- [ ] maybe <kbd>LEFT CTRL</kbd> + <kbd>ENTER</kbd> could repeat the body without the header (basically automatically getting a basic item expansion)
