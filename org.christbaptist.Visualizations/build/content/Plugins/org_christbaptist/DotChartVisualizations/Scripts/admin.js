var dotStyles = {
    "red": "fill: #FF595E;",
    "orange": "fill: rgba(255, 109, 0, 0.60);",
    "yellow": "fill: #FCCB44;",
    "green": "fill: #8FC635;",
    "blue": "fill: #1A6DAD;",
    "purple": "fill: #963484;",
    "outline": "stroke: rgba(255, 0, 0, 0.60);"
}

function displayOptionChosen(forControl) {
    let circleEl = forControl.parentElement.parentElement.parentElement.parentElement.previousElementSibling.previousElementSibling.querySelector('circle');
    let customCSSBox = forControl.parentElement.parentElement.parentElement.nextElementSibling.querySelector('textarea');
    switch (forControl.value) {
        default:
            // Set the custom CSS Box to these styles so they get saved
            customCSSBox.value = dotStyles[forControl.value];
            // But hide the box, because it's not relevant with this selection
            customCSSBox.parentElement.parentElement.parentElement.style.display = "none";
            break;
        case "outline":
            // Set the custom CSS Box to these styles so they get saved
            customCSSBox.value = dotStyles[forControl.value];
        case "custom":
            // Show the custom CSS box
            customCSSBox.parentElement.parentElement.parentElement.style.display = "initial";
            break;
    }

    circleEl.setAttribute('style', customCSSBox.value);
}

function updateCustomStyles(forControl) {
    try {
        let circleEl = forControl.parentElement.parentElement.parentElement.parentElement.previousElementSibling.previousElementSibling.querySelector('circle');
        circleEl.setAttribute('style', forControl.value);
    } catch (error) {
        console.log(error);
    }
}

/**
 * Setup the Style dropdown the first time
 * */
function initializeOptions() {
    //let dropdown = document.querySelector('#' + dropdownId);
    document.querySelectorAll('.sccDvvFilterColorPicker select').forEach((dropdown) => {

        let customCSSBox = dropdown.parentElement.parentElement.parentElement.nextElementSibling.querySelector('textarea');
        let optionFound = false;

        for (let key of Object.keys(dotStyles)) {
            if (customCSSBox.value.trim() == dotStyles[key].trim()) {
                dropdown.querySelector('option[value="' + key + '"]').setAttribute('selected', 'selected');
                optionFound = true;
                break;
            }
        }

        if (!optionFound) {
            dropdown.querySelector('option[value="custom"]').setAttribute('selected', 'selected');
        }

        displayOptionChosen(dropdown);

    });

}

/**
 * Drag handlers
 **/

function dragstart_handler(ev) {
    ev.dataTransfer.setData("application/scc", ev.target.parentElement.parentElement.id);
    ev.dataTransfer.dropEffect = 'move';
}

function dragover_handler(ev) {
    ev.preventDefault();

    console.log(ev.currentTarget.parentElement.parentElement.id, ev.dataTransfer.getData("application/scc"));

    if (ev.clientY < (ev.currentTarget.getBoundingClientRect().y + (ev.currentTarget.offsetHeight / 2))) {
        // Show line on top of element
        if (!ev.currentTarget.classList.contains('drop_top')) {
            document.querySelectorAll('.drop_top').forEach(el => el.classList.remove('drop_top'));
            document.querySelectorAll('.drop_bottom').forEach(el => el.classList.remove('drop_bottom'));
            ev.currentTarget.classList.add('drop_top');
        }
    } else {
        // Show line on bottom of element
        if (!ev.currentTarget.classList.contains('drop_bottom')) {
            document.querySelectorAll('.drop_top').forEach(el => el.classList.remove('drop_top'));
            document.querySelectorAll('.drop_bottom').forEach(el => el.classList.remove('drop_bottom'));
            ev.currentTarget.classList.add('drop_bottom');
        }
    }
    ev.dataTransfer.dropEffect = 'move';
}

function drop_handler(ev) {
    ev.preventDefault();
    document.querySelectorAll('.drop_top').forEach(el => el.classList.remove('drop_top'));
    document.querySelectorAll('.drop_bottom').forEach(el => el.classList.remove('drop_bottom'));
    var data = ev.dataTransfer.getData("application/scc");

    var sourceEl = document.getElementById(ev.dataTransfer.getData('application/scc'));

    // Only allow dropping within the same list in this case
    if (!ev.currentTarget.parentElement.contains(sourceEl)) {
        ev.dataTransfer.dropEffect = 'none';
        return false;
    }

    if (ev.clientY < (ev.currentTarget.getBoundingClientRect().y + (ev.currentTarget.offsetHeight / 2))) {
        // Dropped in top half
        ev.currentTarget.parentElement.insertBefore(sourceEl, ev.currentTarget );
    } else {
        // Dropped in bottom half
        ev.currentTarget.parentElement.insertBefore(sourceEl, ev.currentTarget.nextElementSibling );
    }

    ev.currentTarget.parentElement.querySelectorAll('input[name*="hfSortOrder"]').forEach((el, index) => {
        el.value = index;
    })

}