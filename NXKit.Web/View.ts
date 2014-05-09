﻿/// <reference path="Node.ts" />
/// <reference path="TypedEvent.ts" />

module NXKit.Web {

    /**
     * Main NXKit client-side view class. Injects the view interface into a set of HTML elements.
     */
    export class View {

        private _body: HTMLElement;
        private _server: IServerInvoke;

        private _root: Node;
        private _bind: boolean;
        private _messages: KnockoutObservableArray<Message>;
        private _threshold: Severity;

        private _queue: any[];
        private _queueRunning: boolean;

        private _busy: KnockoutObservable<boolean>;

        constructor(body: HTMLElement, server: IServerInvoke) {
            var self = this;

            self._server = server;
            self._body = body;
            self._root = null;
            self._bind = true;

            self._messages = ko.observableArray<Message>();
            self._threshold = Severity.Warning;

            self._queue = new Array<any>();
            self._queueRunning = false;
            self._busy = ko.observable(false);
        }

        public get Busy(): KnockoutObservable<boolean> {
            return this._busy;
        }

        public get Body(): HTMLElement {
            return this._body;
        }

        public get Root(): Node {
            return this._root;
        }

        public get Messages(): KnockoutObservableArray<Message> {
            return this._messages;
        }

        public get Threshold(): Severity {
            return this._threshold;
        }

        public set Threshold(threshold: Severity) {
            this._threshold = threshold;
        }

        /**
         * Updates the view in response to some received data.
         */
        public Receive(data: any[]) {
            this.Apply(data['Node'] || null);
            this.AppendMessages(data['Messages'] || []);
            this.ExecuteScripts(data['Scripts'] || []);
        }

        /**
         * Updates the messages of the view with the specified items.
         */
        private AppendMessages(messages: any[]) {
            var self = this;

            for (var i = 0; i < messages.length; i++) {

                var severity = <Severity>((<any>Severity)[<string>(messages[i].Severity)]);
                var text = messages[i].Text || '';
                if (severity >= self._threshold)
                    self._messages.push(new Message(severity, text));
            }
        }

        /**
         * Executes the given scripts.
         */
        private ExecuteScripts(scripts: string[]) {
            for (var i = 0; i < scripts.length; i++) {
                var script = scripts[i];
                if (script != null)
                    eval(script);
            }
        }

        /**
         * Removes the current message from the set of messages.
         */
        public RemoveMessage(message: Message) {
            this._messages.remove(message);
        }

        /**
         * Initiates a refresh of the view model.
         */
        private Apply(data: any) {
            var self = this;

            if (self._root == null) {
                // generate new node tree
                self._root = new Node(self, data);
            }
            else {
                // update existing node tree
                self._root.Apply(data);
            }

            self.ApplyBindings();
        }

        /**
         * Invoked when the view model initiates a request to push an update to a node.
         */
        PushUpdate(node: Node, $interface: Interface, property: Property, value: any) {
            var self = this;
            Log.Debug('View.PushUpdate');

            // generate push action
            var data = {
                Action: 'Update',
                Args: {
                    NodeId: node.Id,
                    Interface: $interface.Name,
                    Property: property.Name,
                    Value: value,
                }
            };

            self.Queue(data);
        }

        PushInvoke(node: Node, interfaceName: string, methodName: string, params: any) {
            var self = this;
            Log.Debug('View.PushInvoke');

            // generate push action
            var data = {
                Action: 'Invoke',
                Args: {
                    NodeId: node.Id,
                    Interface: interfaceName,
                    Method: methodName,
                    Params: params,
                }
            };

            self.Queue(data);
        }

        /**
         * Queues the given data to be sent to the server.
         */
        Queue(command: any) {
            var self = this;

            // pushes a new action onto the queue
            self._queue.push(command);

            // only one runner at a time
            if (self._queueRunning) {
                return;
            } else {
                self._queueRunning = true;

                // delay processing in case of new commands
                setTimeout(() => {
                    self._busy(true);

                    // recursive call to work queue
                    var l = () => {
                        var commands = self._queue.splice(0);
                        if (commands.length > 0) {
                            self._server(commands, (data: any) => {

                                // only update node data if no outstanding commands
                                if (self._queue.length == 0) {
                                    self.Apply(data['Node'] || null);
                                }

                                // display messages and execute scripts
                                self.AppendMessages(data['Messages'] || []);
                                self.ExecuteScripts(data['Scripts'] || []);

                                // recurse
                                l();
                            });
                        } else {
                            self._queueRunning = false;
                            self._busy(false);
                        }
                    };

                    // initiate queue run
                    l();
                }, 500);
            }
        }

        /**
         * Applies the bindings to the view if possible.
         */
        ApplyBindings() {
            var self = this;

            // apply bindings to our element and our view model
            if (self._bind &&
                self._body != null &&
                self._root != null) {

                // clear existing bindings
                ko.cleanNode(
                    self._body);

                // ensure body is to render template
                $(self._body)
                    .attr('data-bind', 'template: { name: \'NXKit.View\' }');

                // apply knockout to view node
                ko.applyBindings(
                    self,
                    self._body);

                self._bind = false;
            }

        }

    }

}
