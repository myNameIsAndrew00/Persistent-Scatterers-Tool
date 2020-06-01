/*!Component: List drawer
 *
 * Handles list drawing for a template layout
 *
 * */

export class ListDrawer {

    /**
     * An instance which manage generic list drawing
     * @param {any} properties properties used by drawer to draw data on screen
     * @param {boolean} isVerbose a boolean which indiquate if logs should be made in console or not
     */
    constructor(properties, isVerbose) {
        this.properties = properties;

        this.currentPage = 0;

        this.data = [];

        this.drawList();

        this.isVerbose = isVerbose;

    }

    Draw() {
        this.requestItems();
    }

/** private region */

    drawList() {
        var self = this;

        this.listContainer = $('<div></div>');
        this.listContainer.addClass(this.properties.class);
        this.listContainer.css('display', 'flex');
        this.listContainer.css('flex-direction',
                                this.properties.orientation === 'vertical' ? 'column' : 'row');

        this.listContainer.on('scroll', function () {
            if (self.listContainer.scrollTop() + self.listContainer.innerHeight() >= self.listContainer[0].scrollHeight) {
                self.requestItems();
            }
        });

        $(this.properties.container).append(this.listContainer);
    }

    requestItems() {
        var self = this;

        $.when(this.properties.dataSourceCallback(
            this.currentPage,
            this.properties.itemsPerPage
        )).then(
            function (data) {
                self.data = data;
                self.drawListItems();

                self.currentPage++;
            }
        );
    }

    drawListItems() {
        if (this.isVerbose) console.log(this.data);

        for (var i = 0; i < this.data.length; i++) {
            var listItem = this.properties.onItemCreate(this.data[i], this.currentPage * this.properties.itemsPerPage + i);

            this.listContainer.append(listItem);
        }
    }
}



/*
    var drawer = new ListDrawer({
       container: containerId,                                  --id of the container which contain the list
       class: class                                             --class of list (styling)
       itemsPerPage: 10,                                        --items loaded at once
       onItemCreate: function(itemValue, itemIndex),            --function which is called to render a card(returns an html element);
       dataSourceCallback: function(pageIndex, itemsPerPage),   --function which is called to provide data
       orientation: 'vertical'/'horizontal'                     --orientation of the list 
    });

 */