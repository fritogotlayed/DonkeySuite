/*
package com.maddonkeysoftware.donkeydesktopmonitor.model;

import static org.junit.Assert.*;
import static org.mockito.Mockito.*;

import java.io.File;
import java.lang.reflect.Constructor;
import java.lang.reflect.InvocationTargetException;
import java.util.Arrays;

import org.hibernate.Session;
import org.junit.*;
import org.springframework.context.ApplicationContext;
import org.springframework.context.support.ClassPathXmlApplicationContext;

import com.maddonkeysoftware.donkeydesktopmonitor.ContextProvider;
import com.maddonkeysoftware.donkeydesktopmonitor.MockBeanProvider;
import com.maddonkeysoftware.donkeydesktopmonitor.model.settings.OperationMode;
import com.maddonkeysoftware.donkeydesktopmonitor.model.settings.WatchDirectory;

public class WatchedDirectoryTest {
    @Before
    public void setUp() throws Exception {
        com.maddonkeysoftware.donkeydesktopmonitor.ContextProvider
                .setContext(new ClassPathXmlApplicationContext("testApplicationContext.xml"));
    }
    
    @After
    public void tearDown() throws Exception {
        System.gc();
        MockBeanProvider.clear();
    }

    @Test
    public final void watchedDirectorySetAndGetWorkProperly() {

        // Arrange
        String val = "C:\\Directory";
        String extensions = "jpg,jpeg";
        WatchedDirectory dir = new WatchedDirectory();
        WatchDirectory dirConfig = new WatchDirectory();
        dirConfig.setPath(val);
        dirConfig.setFileExtensions(extensions);
        dirConfig.setMode(OperationMode.UploadOnly);
        dirConfig.setIncludeSubDirectories(true);

        // Act
        dir.configure(dirConfig);

        // Assert
        assertEquals(val, dir.getDirectory());
        assertEquals(true, dir.getSubDirectoriesIncluded());
        assertEquals(OperationMode.UploadOnly, dir.getMode());
        assertEquals(new java.util.ArrayList<String>(Arrays.asList(extensions.split(","))), dir.getAcceptableExtensions());
    }
    
    @Test
    public final void watchedDirectoryProcessAvailableImagesDoesNothingWhenNoImagesFound() {

        // Arrange
        String val = "C:\\Directory";
        String extensions = "jpg,jpeg";
        WatchedDirectory dir = new WatchedDirectory();
        WatchDirectory dirConfig = new WatchDirectory();
        dirConfig.setPath(val);
        dirConfig.setFileExtensions(extensions);
        dirConfig.setMode(OperationMode.UploadOnly);
        dirConfig.setIncludeSubDirectories(true);
        dirConfig.setSortStrategy("Simple");
        dir.configure(dirConfig);

        Session mockSession = mock(Session.class);
        File mockCurrentLocation = mock(File.class);
        when(mockCurrentLocation.listFiles()).thenReturn(new File[0]);
        MockBeanProvider.enqueueOneArgFile(mockCurrentLocation);

        // Act
        dir.processAvailableImages(mockSession);

        // Assert
        assertEquals(val, dir.getDirectory());
        assertEquals(true, dir.getSubDirectoriesIncluded());
        assertEquals(OperationMode.UploadOnly, dir.getMode());
        assertEquals(new java.util.ArrayList<String>(Arrays.asList(extensions.split(","))), dir.getAcceptableExtensions());
        // verify(mockSession);
        verify(mockCurrentLocation).listFiles();
    }
    
//    @Test
//   public final void contextProvider_ReturnsProvidedContextWhenAvailable() {
//      ClassPathXmlApplicationContext testContext = new ClassPathXmlApplicationContext("testApplicationContext.xml");

//      // Act
//      ApplicationContext obj1 = ContextProvider.getContext();
//      com.maddonkeysoftware.donkeydesktopmonitor.ContextProvider.setContext(testContext);
//      ApplicationContext obj2 = ContextProvider.getContext();
//      
//      // Assert
//      assertNotEquals(obj1, obj2);
//      assertEquals(testContext, obj2);
//  }

//  @Test(expected = InvocationTargetException.class)
//  public final void sessionFactoryProvider_ConstructorThrowsUnsupportedException() 
//          throws InstantiationException, IllegalAccessException, IllegalArgumentException, InvocationTargetException, 
//          NoSuchMethodException, SecurityException {

//      Constructor<ContextProvider> c = ContextProvider.class.getDeclaredConstructor();
//      c.setAccessible(true);
//      ContextProvider u = c.newInstance();

//  }
}
*/