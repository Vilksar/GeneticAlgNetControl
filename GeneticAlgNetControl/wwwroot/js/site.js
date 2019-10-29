// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your Javascript code.
$(window).on('load', () => {

    // Check if there is a dropdown menu form on the page.
    if ($('.dropdown-menu-form').length !== 0) {
        // Add a listener for licking inside the dropdown menu.
        $('.dropdown-menu.dropdown-menu-form').on('click', (event) => {
            // Stop the propagation.
            event.stopPropagation();
        });
    }

    // Check if there is a list group of items on the page.
    if ($('.item-group').length !== 0) {
        // Define a function which gets all of the selected items and creates a JSON string array with their IDs.
        const updateSelectedItems = (groupElement) => {
            // Get all of the list group items.
            const items = $(groupElement).find('.item-group-item');
            // Remove the active class from all list items.
            $(items).removeClass('active');
            // Go over all of the checked elements and get the corresponding list group items.
            const selectedItems = $(groupElement).find('input[type="checkbox"]:checked').closest('.item-group-item');
            // Go over each of the selected items and mark them as active.
            $(selectedItems).addClass('active');
            // Check how many elements are selected.
            if (selectedItems.length === 0) {
                // Disable the group buttons.
                $('.item-group-button').prop('disabled', true);
                // Unmark the checkbox as indeterminate.
                $(groupElement).find('.item-group-select').prop('indeterminate', false);
                // Uncheck the checkbox.
                $(groupElement).find('.item-group-select').prop('checked', false);
            } else {
                // Enable the group buttons.
                $('.item-group-button').prop('disabled', false);
                // Check if not all elements are selected.
                if (selectedItems.length !== items.length) {
                    // Mark the checkbox as indeterminate.
                    $(groupElement).find('.item-group-select').prop('indeterminate', true);
                } else {
                    // Unmark the checkbox as indeterminate.
                    $(groupElement).find('.item-group-select').prop('indeterminate', false);
                    // Check the checkbox.
                    $(groupElement).find('.item-group-select').prop('checked', true);
                }
            }
        };
        // Add a listener for when a checkbox gets checked or unchecked.
        $('.item-group').on('change', 'input[type="checkbox"]', (event) => {
            // Get the current list group.
            const groupElement = $(event.target).closest('.item-group');
            // Update the selected items.
            updateSelectedItems(groupElement);
        });
        // Add a listener for the select checkbox.
        $('.item-group-select').on('change', (event) => {
            // Get the current list group.
            const groupElement = $(event.target).closest('.item-group');
            // Check if the checkbox is currently checked.
            if ($(event.target).prop('checked')) {
                // Check all of the checkboxes on the page.
                $(groupElement).find('input[type="checkbox"]:not(:checked)').prop('checked', true);
            } else {
                // Uncheck all of the checkboxes on the page.
                $(groupElement).find('input[type="checkbox"]:checked').prop('checked', false);
            }
            // Update the selected items.
            updateSelectedItems(groupElement);
        });
        // On page load, parse the input and check the group items.
        (() => {
            // Go over all of the groups.
            $('.item-group').each((index, element) => {
                // Update the selected items.
                updateSelectedItems(element);
            });
        })();
    }

    // Check if there is a file protein group on the page.
    if ($('.file-group').length !== 0) {
        // Define a function which updates the data to be submitted.
        const updateText = (groupElement) => {
            // Check if the text is empty.
            if (!$.trim($(groupElement).find('.file-group-text').first().val())) {
                // Update the value of the count.
                $(groupElement).find('.file-group-count').first().text(0);
                // Update the data to be submitted.
                $(groupElement).find('.file-group-input').first().val(JSON.stringify([]));
                // Return from the function.
                return;
            }
            // Get the in-line separator.
            const inlineSeparator = $(groupElement).find('.file-group-in-line-separator').first().val();
            // Get the line separator.
            const lineSeparator = $(groupElement).find('.file-group-line-separator').first().val();
            // Get the type of the file group.
            const type = $(groupElement).data('type');
            // Check if we have nodes.
            if (type === 'nodes') {
                // Split the text into different lines.
                const rows = $(groupElement).find('.file-group-text').first().val().split(new RegExp(lineSeparator)).filter((element) => {
                    // Select only the non empty elements.
                    return $.trim(element);
                });
                // Update the value of the count.
                $(groupElement).find('.file-group-count').first().text(rows.length);
                // Update the data to be submitted.
                $(groupElement).find('.file-group-input').first().val(JSON.stringify(rows));
            }
            // Check if we have interactions.
            else if (type === 'edges') {
                // Split the text into different lines.
                const rows = $(groupElement).find('.file-group-text').first().val().split(new RegExp(lineSeparator)).filter((element) => {
                    // Split the row into its composing items.
                    var row = element.split(new RegExp(inlineSeparator)).filter((el) => {
                        // Select only the non-empty items.
                        return el !== '';
                    });
                    // Select only elements with more than two items.
                    return row.length === 2;
                });
                // Go over each row.
                const items = $.map(rows, (element, index) => {
                    // Split the row into its composing items.
                    var row = element.split(new RegExp(inlineSeparator));
                    // Check if we don't have both source and target nodes.
                    if (!row[0] || !row[1]) {
                        // Don't return anything.
                        return;
                    }
                    // Split the element into an array of items.
                    return { "SourceNode": row[0], "TargetNode": row[1] };
                });
                // Update the value of the count.
                $(groupElement).find('.file-group-count').first().text(rows.length);
                // Update the data to be submitted.
                $(groupElement).find('.file-group-input').first().val(JSON.stringify(items));
            }
        };
        // Add a listener for typing into the text area.
        $('.file-group-text').on('keyup', (event) => {
            // Get the actual group which was clicked.
            const groupElement = $(event.target).closest('.file-group');
            // Update the selected items.
            updateText(groupElement);
        });
        // Add a listener for changing one of the separators.
        $('.file-group').on('change', '.file-group-separator', (event) => {
            // Get the actual group which was clicked.
            const groupElement = $(event.target).closest('.file-group');
            // Update the selected items.
            updateText(groupElement);
        });
        // Add a listener for if the upload button was clicked.
        $('.file-group-file-upload').on('change', (event) => {
            // Get the current file.
            const file = event.target.files[0];
            // Set the filename in the label.
            $(event.target).siblings('.file-group-file-label').html(file.name);
            // Define the file reader and the variable for storing its content.
            let fileReader = new FileReader();
            // Define what happens when we read the file.
            fileReader.onload = (e) => {
                // Write the content of the file to the text area.
                $(event.target).closest('.file-group').find('.file-group-text').first().val(e.target.result);
                // Get the actual group which was clicked.
                const groupElement = $(event.target).closest('.file-group');
                // Update the selected items.
                updateText(groupElement);
            };
            // Read the file.
            fileReader.readAsText(file);
        });
        // On page load, parse the input and add the group items.
        (() => {
            // Go over all of the groups.
            $('.file-group').each((index, groupElement) => {
                // Get the type of the file group.
                const type = $(groupElement).data('type');
                // Define a variable for the input data.
                let data = undefined;
                // Try to parse the input data.
                try {
                    // Get the input data.
                    data = JSON.parse($(groupElement).find('.file-group-input').first().val());
                }
                catch (error) {
                    // Return from the function.
                    return;
                }
                // Check if there isn't any data.
                if (typeof data === 'undefined') {
                    // Return from the function.
                    return;
                }
                // Check if we have a proper array.
                if (!Array.isArray(data)) {
                    // Return from the function.
                    return;
                }
                // Check if we have nodes.
                if (type === 'nodes') {
                    // Go over all of the elements.
                    data = data.filter((element) => {
                        // Keep only the ones which are of the proper type.
                        return typeof element === 'string';
                    });
                    // Add the elements to the text.
                    $(groupElement).find('.file-group-text').first().val(data.join('\n'));
                }
                // Check if we have edges.
                if (type === 'edges') {
                    // Go over all of the elements.
                    data = data.filter((element) => {
                        // Keep only the ones which are of the proper type.
                        return element['SourceNode'] && typeof element['SourceNode'] === 'string' && element['TargetNode'] && typeof element['TargetNode'] === 'string';
                    });
                    // Go over all of the elements.
                    data = $.map(data, (element, index) => {
                        // Check if we have more than one of each.
                        if (!element['SourceNode'] || !element['TargetNode']) {
                            // Return an undefined value.
                            return undefined;
                        } else {
                            // Return a simplified object.
                            return `${element['SourceNode']};${element['TargetNode']}`;
                        }
                    });
                    // Add the elements to the text.
                    $(groupElement).find('.file-group-text').first().val(data.join('\n'));
                }
                // Update the selected items.
                updateText(groupElement);
            });
        })();
    }

    // Check if there is any algorithm item on page.
    if ($('.algorithm-item-status').length !== 0) {
        // Define a function which updates all algorithms on page.
        const updateAlgorithms = () => {
            // Go over each algorithm item element on the page.
            $('.algorithm-item-status').each((index, element) => {
                // Get its current status.
                let status = $(element).text();
                // Check if the algorithm needs updating.
                if (status === '' || status === 'Scheduled' || status === 'Ongoing' || status === 'ScheduledToStop') {
                    // Get the parent element.
                    const parent = $(element).closest('.item-group-item');
                    // Get the ID.
                    const id = $(parent).find('.algorithm-item-id').first().text();
                    // Retrieve the new data for the algorithm with the mentioned ID.
                    const json = $.ajax({
                        url: `/Overview?handler=Refresh&id=${id}`,
                        async: false,
                        dataType: 'json'
                    }).responseJSON;
                    // Update the corresponding fields.
                    $(parent).find('.algorithm-item-status').attr('title', json.statusTitle);
                    $(parent).find('.algorithm-item-status').text(json.statusText);
                    $(parent).find('.algorithm-item-time-span').attr('title', json.timeSpanTitle);
                    $(parent).find('.algorithm-item-time-span').text(json.timeSpanText);
                    $(parent).find('.algorithm-item-progress-iterations').attr('title', json.progressIterationsTitle);
                    $(parent).find('.algorithm-item-progress-iterations').text(json.progressIterationsText);
                    $(parent).find('.algorithm-item-progress-iterations-without-improvement').attr('title', json.progressIterationsWithoutImprovementTitle);
                    $(parent).find('.algorithm-item-progress-iterations-without-improvement').text(json.progressIterationsWithoutImprovementText);
                    // Check if the status has been updated.
                    if (status !== json.statusText) {
                        // Update the status.
                        status = json.statusText;
                        // Check the new status.
                        if (status === 'Scheduled') {
                            // Hide and show the corresponding buttons.
                            $(parent).find('.algorithm-item-button-start').addClass('d-none');
                            $(parent).find('.algorithm-item-button-stop').addClass('d-none');
                            $(parent).find('.algorithm-item-button-save').addClass('d-none');
                            $(parent).find('.algorithm-item-button-delete').removeClass('d-none');
                        } else if (status === 'Ongoing') {
                            // Hide and show the corresponding buttons.
                            $(parent).find('.algorithm-item-button-start').addClass('d-none');
                            $(parent).find('.algorithm-item-button-stop').removeClass('d-none');
                            $(parent).find('.algorithm-item-button-save').addClass('d-none');
                            $(parent).find('.algorithm-item-button-delete').addClass('d-none');
                        } else if (status === 'ScheduledToStop') {
                            // Hide and show the corresponding buttons.
                            $(parent).find('.algorithm-item-button-start').addClass('d-none');
                            $(parent).find('.algorithm-item-button-stop').addClass('d-none');
                            $(parent).find('.algorithm-item-button-save').addClass('d-none');
                            $(parent).find('.algorithm-item-button-delete').addClass('d-none');
                        } else if (status === 'Stopped') {
                            // Hide and show the corresponding buttons.
                            $(parent).find('.algorithm-item-button-start').removeClass('d-none');
                            $(parent).find('.algorithm-item-button-stop').addClass('d-none');
                            $(parent).find('.algorithm-item-button-save').removeClass('d-none');
                            $(parent).find('.algorithm-item-button-delete').removeClass('d-none');
                        } else if (status === 'Completed') {
                            // Hide and show the corresponding buttons.
                            $(parent).find('.algorithm-item-button-start').addClass('d-none');
                            $(parent).find('.algorithm-item-button-stop').addClass('d-none');
                            $(parent).find('.algorithm-item-button-save').removeClass('d-none');
                            $(parent).find('.algorithm-item-button-delete').removeClass('d-none');
                        }
                    }
                }
            });
        };
        // Update the algorithms on page load.
        updateAlgorithms();
        // Repeat the function every 5 seconds.
        setInterval(function () {
            // Update the algorithms.
            updateAlgorithms();
        }, 5000);
    }

    // Check if there is any algorithm whose details to refresh on page.
    if ($('.algorithm-details-status').length !== 0) {
        // Get the current status.
        const currentStatus = $('.algorithm-details-status').val();
        // Check if the page needs updating.
        if (currentStatus === 'Scheduled' || currentStatus === 'Ongoing' || currentStatus === 'ScheduledToStop') {
            // Repeat the function every 5 seconds.
            setInterval(function () {
                // Get the ID of the algorithm.
                let id = $('.algorithm-details-id').val();
                // Retrieve the new data for the algorithm with the mentioned ID.
                let json = $.ajax({
                    url: `/Details?handler=Refresh&id=${id}`,
                    async: false,
                    dataType: 'json'
                }).responseJSON;
                // Check if the algorithm has ended.
                if (json.status === 'Stopped' || json.status === 'Completed') {
                    // Reload the page.
                    location.reload(true);
                }
                // Update the corresponding fields.
                $('.algorithm-details-current-iteration').val(json.currentIteration);
                $('.algorithm-details-current-iteration-without-improvement').val(json.currentIterationWithoutImprovement);
            }, 5000);
        }
    }
});