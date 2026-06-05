-- Schema for Online Doctor Consultation & Chat System
-- Requires PostgreSQL extensions for UUID generation
CREATE EXTENSION IF NOT EXISTS pgcrypto;

-- USERS
CREATE TABLE IF NOT EXISTS users (
    id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    full_name varchar(150) NOT NULL,
    email varchar(150) NOT NULL UNIQUE,
    password_hash varchar(200) NOT NULL,
    role varchar(50) NOT NULL,
    phone varchar(20),
    created_at timestamptz NOT NULL DEFAULT now(),
    updated_at timestamptz
);

-- DOCTOR PROFILES
CREATE TABLE IF NOT EXISTS doctor_profiles (
    id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id uuid NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    specialization varchar(100) NOT NULL,
    bio text,
    consultation_fee numeric(10,2) DEFAULT 0,
    is_available boolean DEFAULT true,
    created_at timestamptz NOT NULL DEFAULT now(),
    updated_at timestamptz
);

-- AVAILABILITY SLOTS
CREATE TABLE IF NOT EXISTS availability_slots (
    id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    doctor_id uuid NOT NULL REFERENCES doctor_profiles(id) ON DELETE CASCADE,
    date date NOT NULL,
    start_time time NOT NULL,
    end_time time NOT NULL,
    is_booked boolean DEFAULT false,
    created_at timestamptz NOT NULL DEFAULT now(),
    updated_at timestamptz
);

-- APPOINTMENTS
CREATE TYPE appointment_status AS ENUM ('Pending','Confirmed','InProgress','Completed','Cancelled');

CREATE TABLE IF NOT EXISTS appointments (
    id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    patient_id uuid NOT NULL REFERENCES users(id),
    doctor_id uuid NOT NULL REFERENCES doctor_profiles(id),
    slot_id uuid NOT NULL REFERENCES availability_slots(id),
    status appointment_status NOT NULL DEFAULT 'Pending',
    notes text,
    created_at timestamptz NOT NULL DEFAULT now(),
    updated_at timestamptz
);

-- CONSULTATION SESSIONS
CREATE TABLE IF NOT EXISTS consultation_sessions (
    id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    appointment_id uuid NOT NULL REFERENCES appointments(id),
    started_at timestamptz NOT NULL,
    ended_at timestamptz,
    summary text,
    created_at timestamptz NOT NULL DEFAULT now(),
    updated_at timestamptz
);

-- CHAT MESSAGES
CREATE TABLE IF NOT EXISTS chat_messages (
    id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    session_id uuid NOT NULL REFERENCES consultation_sessions(id) ON DELETE CASCADE,
    sender_id uuid NOT NULL REFERENCES users(id),
    message text NOT NULL,
    sent_at timestamptz NOT NULL DEFAULT now(),
    is_read boolean DEFAULT false
);

-- NOTIFICATIONS
CREATE TABLE IF NOT EXISTS notifications (
    id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id uuid NOT NULL REFERENCES users(id),
    title varchar(200) NOT NULL,
    message text NOT NULL,
    type varchar(50) NOT NULL,
    is_read boolean DEFAULT false,
    created_at timestamptz NOT NULL DEFAULT now()
);

-- REVIEWS
CREATE TABLE IF NOT EXISTS reviews (
    id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    appointment_id uuid NOT NULL REFERENCES appointments(id),
    patient_id uuid NOT NULL REFERENCES users(id),
    rating int NOT NULL CHECK (rating >= 1 AND rating <= 5),
    comment text,
    created_at timestamptz NOT NULL DEFAULT now()
);

-- Indexes
CREATE INDEX IF NOT EXISTS ix_appointments_patient_id ON appointments(patient_id);
CREATE INDEX IF NOT EXISTS ix_appointments_doctor_id ON appointments(doctor_id);
CREATE INDEX IF NOT EXISTS ix_appointments_status ON appointments(status);
CREATE INDEX IF NOT EXISTS ix_users_email ON users(email);

-- Additional helpful indexes
CREATE INDEX IF NOT EXISTS ix_availability_slots_doctor_date ON availability_slots(doctor_id, date);
CREATE INDEX IF NOT EXISTS ix_notifications_user_id ON notifications(user_id);
