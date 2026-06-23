-- Seed data for Online Doctor Consultation & Chat System
-- Uses gen_random_uuid() from pgcrypto

-- Create two specializations as distinct doctor profiles
-- Create patients and doctors

-- Patients
INSERT INTO users (id, full_name, email, password_hash, role, phone, created_at)
VALUES
  (gen_random_uuid(), 'Patient One', 'patient1@test.com', '$2a$12$eIXqH1Jv1D9Yw9uQm3Z6.ejKq9Yq0uQxX6e7qk1pFjYc8h6QZrG6O', 'Patient', '555-0101', now()),
  (gen_random_uuid(), 'Patient Two', 'patient2@test.com', '$2a$12$eIXqH1Jv1D9Yw9uQm3Z6.ejKq9Yq0uQxX6e7qk1pFjYc8h6QZrG6O', 'Patient', '555-0102', now());

-- Doctors (users)
INSERT INTO users (id, full_name, email, password_hash, role, phone, created_at)
VALUES
  (gen_random_uuid(), 'Doctor One', 'doctor1@test.com', '$2a$12$eIXqH1Jv1D9Yw9uQm3Z6.ejKq9Yq0uQxX6e7qk1pFjYc8h6QZrG6O', 'Doctor', '555-0201', now()),
  (gen_random_uuid(), 'Doctor Two', 'doctor2@test.com', '$2a$12$eIXqH1Jv1D9Yw9uQm3Z6.ejKq9Yq0uQxX6e7qk1pFjYc8h6QZrG6O', 'Doctor', '555-0202', now()),
  (gen_random_uuid(), 'Doctor Three', 'doctor3@test.com', '$2a$12$eIXqH1Jv1D9Yw9uQm3Z6.ejKq9Yq0uQxX6e7qk1pFjYc8h6QZrG6O', 'Doctor', '555-0203', now());

-- Insert doctor profiles (3 doctors)
INSERT INTO doctor_profiles (id, user_id, specialization, bio, consultation_fee, is_available, created_at)
SELECT gen_random_uuid(), u.id, 'Cardiology', 'Experienced cardiologist', 120.00, true, now()
FROM users u WHERE u.email = 'doctor1@test.com';

INSERT INTO doctor_profiles (id, user_id, specialization, bio, consultation_fee, is_available, created_at)
SELECT gen_random_uuid(), u.id, 'Dermatology', 'Skin specialist with 10 years experience', 90.00, true, now()
FROM users u WHERE u.email = 'doctor2@test.com';

INSERT INTO doctor_profiles (id, user_id, specialization, bio, consultation_fee, is_available, created_at)
SELECT gen_random_uuid(), u.id, 'Pediatrics', 'Child health specialist', 80.00, true, now()
FROM users u WHERE u.email = 'doctor3@test.com';

-- Create availability slots for doctors (5 slots total)
-- Use the first three doctors inserted above
INSERT INTO availability_slots (id, doctor_id, date, start_time, end_time, is_booked, created_at)
SELECT gen_random_uuid(), dp.id, CURRENT_DATE + INTERVAL '1 day', '09:00'::time, '09:30'::time, false, now()
FROM doctor_profiles dp LIMIT 1;

INSERT INTO availability_slots (id, doctor_id, date, start_time, end_time, is_booked, created_at)
SELECT gen_random_uuid(), dp.id, CURRENT_DATE + INTERVAL '1 day', '10:00'::time, '10:30'::time, false, now()
FROM doctor_profiles dp LIMIT 1 OFFSET 1;

INSERT INTO availability_slots (id, doctor_id, date, start_time, end_time, is_booked, created_at)
SELECT gen_random_uuid(), dp.id, CURRENT_DATE + INTERVAL '2 day', '11:00'::time, '11:30'::time, false, now()
FROM doctor_profiles dp LIMIT 1 OFFSET 2;

-- Two additional slots for first doctor
INSERT INTO availability_slots (id, doctor_id, date, start_time, end_time, is_booked, created_at)
SELECT gen_random_uuid(), dp.id, CURRENT_DATE + INTERVAL '3 day', '14:00'::time, '14:30'::time, false, now()
FROM doctor_profiles dp LIMIT 1;

INSERT INTO availability_slots (id, doctor_id, date, start_time, end_time, is_booked, created_at)
SELECT gen_random_uuid(), dp.id, CURRENT_DATE + INTERVAL '3 day', '15:00'::time, '15:30'::time, false, now()
FROM doctor_profiles dp LIMIT 1;

-- Note: Password hashes above are placeholders using BCrypt format. Replace with secure hashes as needed.
