using System;
using ObjCRuntime;

namespace binding
{
    [Introduced (PlatformName.iOS, 10, 0)]
    [Introduced (PlatformName.MacOSX, 10, 0)]
    public interface Class1
    {
        void Foo ();
        
        [Introduced (PlatformName.iOS, 11, 0)]
        [Introduced (PlatformName.MacOSX, 11, 0)]
        void Bar ();

        [Introduced (PlatformName.iOS, 12, 0)]
        [NoMac]
        void Buzz ();

        [NoiOS]
        int One { get; set; }        
    }
}
