﻿/*
Copyright 2011 Google Inc

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Google.Adsense.Win.Gadget.Commands
{
    internal static class GadgetCommands
    {
        private static readonly RoutedUICommand refresh;

        static GadgetCommands()
        {
            refresh = new RoutedUICommand(
                "Fetches the data from the current AdSenseService again", "Refresh", typeof(GadgetCommands));
        }

        public static RoutedUICommand Refresh { get { return refresh; } }
    }
}
