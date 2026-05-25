package com.verstappen.memorypalace.controller;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RestController;

@RestController
public class HelloController {
    // Logger declaration (CLASS LEVEL)

    private static final Logger logger = LoggerFactory.getLogger(HelloController.class);

    @GetMapping("/")
    public String home() {
        // Logging inside methodd
        logger.info("Home endpoint was called");
        return "Memory Palace Backend is Running";
    }

}
