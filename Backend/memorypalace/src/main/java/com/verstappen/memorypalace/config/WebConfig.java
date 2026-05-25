package com.verstappen.memorypalace.config;

import org.springframework.context.annotation.Configuration;
import org.springframework.web.servlet.config.annotation.CorsRegistry;
import org.springframework.web.servlet.config.annotation.WebMvcConfigurer;

/**
 * Allows the Memory Palace frontend (served from any origin during development)
 * to call the REST API endpoints.  When the HTML is served directly by this
 * Spring Boot application (i.e., from src/main/resources/static/) the same
 * origin applies automatically and CORS is not strictly required, but this
 * config keeps things working even when the HTML is opened from the filesystem
 * or served by a separate dev server.
 */
@Configuration
public class WebConfig implements WebMvcConfigurer {

    @Override
    public void addCorsMappings(CorsRegistry registry) {
        registry.addMapping("/**")
                .allowedOriginPatterns("*")
                .allowedMethods("GET", "POST", "PUT", "PATCH", "DELETE", "OPTIONS")
                .allowedHeaders("*");
    }
}
