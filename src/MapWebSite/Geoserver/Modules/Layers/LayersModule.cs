
using MapWebSite.GeoserverAPI.Modules.Layers;
using MapWebSite.Types;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace MapWebSite.GeoserverAPI.Modules
{
    internal class LayersModule : IGeoserverModule
    {
        //a field which specify if the module should be used only for one layer
        private readonly LayersBuilder layersBuilder = null;

        public LayersModule(LayersBuilder layersBuilder)
        {
            this.layersBuilder = layersBuilder;
        }

        public HttpContent GetContent()
        {
            StringContent content = new StringContent(layersBuilder.ToXml(),
                                                        Encoding.UTF8,
                                                        "application/xml");

            return content;
        }

        public string GetEndpoint()
        {
            return layersBuilder.SingleLayer ?
                (string.IsNullOrEmpty(layersBuilder.Workspace) ?
                    string.Format(Endpoints.Layer.GetEnumString(), this.layersBuilder.LayerName) :
                    string.Format(Endpoints.WorkspaceLayer.GetEnumString(), this.layersBuilder.Workspace, this.layersBuilder.LayerName)) :
                (string.IsNullOrEmpty(layersBuilder.Workspace) ?
                    Endpoints.Layers.GetEnumString() :
                    string.Format(Endpoints.WorkspaceLayers.GetEnumString(), this.layersBuilder.Workspace));
        }

    }
}
